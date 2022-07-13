#
#  -------------------------------------------------------------
#   Copyright (c) Microsoft Corporation.  All rights reserved.
#  -------------------------------------------------------------
"""
Skeleton code showing how to load and run the ONNX export package from Lobe.
"""
from azureml.core.model import Model
from azureml.contrib.services.aml_request import AMLRequest, rawhttp
from azureml.contrib.services.aml_response import AMLResponse
from PIL import Image
import json
import os
import io
import base64
import pandas as pd
import numpy as np
from PIL import Image
import onnxruntime as rt
import logging

from inference_schema.parameter_types.pandas_parameter_type import PandasParameterType
from inference_schema.schema_decorators import input_schema, output_schema

logging.basicConfig(level=logging.DEBUG)

sample_input = PandasParameterType(pd.DataFrame([{'image': 'xd8\xe1\xb7\xeb\xa8\xe5 \xd2\xb7\xe1'}]))
sample_prediction = {"Prediction": "NORMAL", "Confidence": 0.9}
sample_output_schema = PandasParameterType(pd.DataFrame([sample_prediction]))

def get_model_and_sig(model_dir):
    """Method to get name of model file. Assumes model is in the parent directory for script."""
    with open(os.path.join(model_dir, "signature.json"), "r") as f:
        signature = json.load(f)
    model_file = os.path.join(model_dir, signature.get("filename"))
    if not os.path.isfile(model_file):
        raise FileNotFoundError(f"Model file does not exist")
    return model_file, signature


def load_model(model_file):
    """Load the model from path to model file"""
    # Load ONNX model as session.
    return rt.InferenceSession(path_or_bytes=model_file)


def get_prediction(image, session, signature):
    """
    Predict with the ONNX session!
    """
    # get the signature for model inputs and outputs
    signature_inputs = signature.get("inputs")
    signature_outputs = signature.get("outputs")

    if "Image" not in signature_inputs:
        raise ValueError("ONNX model doesn't have 'Image' input! Check signature.json, and please report issue to Lobe.")

    # process image to be compatible with the model
    img = process_image(image, signature_inputs.get("Image").get("shape"))

    # run the model!
    fetches = [(key, value.get("name")) for key, value in signature_outputs.items()]
    # make the image a batch of 1
    feed = {signature_inputs.get("Image").get("name"): [img]}
    outputs = session.run(output_names=[name for (_, name) in fetches], input_feed=feed)
    # un-batch since we ran an image with batch size of 1,
    # convert to normal python types with tolist(), and convert any byte strings to normal strings with .decode()
    results = {}
    for i, (key, _) in enumerate(fetches):
        val = outputs[i].tolist()[0]
        if isinstance(val, bytes):
            val = val.decode()
        if key == "Confidences":
            results["Confidence"] = max(val)
        else:
            results[key] = val

    return results


def process_image(image, input_shape):
    """
    Given a PIL Image, center square crop and resize to fit the expected model input, and convert from [0,255] to [0,1] values.
    """
    width, height = image.size
    # ensure image type is compatible with model and convert if not
    if image.mode != "RGB":
        image = image.convert("RGB")
    # center crop image (you can substitute any other method to make a square image, such as just resizing or padding edges with 0)
    if width != height:
        square_size = min(width, height)
        left = (width - square_size) / 2
        top = (height - square_size) / 2
        right = (width + square_size) / 2
        bottom = (height + square_size) / 2
        # Crop the center of the image
        image = image.crop((left, top, right, bottom))
    # now the image is square, resize it to be the right shape for the model input
    input_width, input_height = input_shape[1:3]
    if image.width != input_width or image.height != input_height:
        image = image.resize((input_width, input_height))

    # make 0-1 float instead of 0-255 int (that PIL Image loads by default)
    image = np.asarray(image) / 255.0
    # format input as model expects
    return image.astype(np.float32)


def init():
    """
    Load the model and signature files, start the ONNX session, and run prediction on the image.
    Output prediction will be a dictionary with the same keys as the outputs in the signature.json file.
    """
    
    global signature
    global session
    model_dir = Model.get_model_path(model_name="PneumoniaDetection-secure")
    files = os.listdir(model_dir)
    for f in files:
        print(os.path.join(model_dir, f))

    model_file, signature = get_model_and_sig(model_dir)
    session = load_model(model_file)

@input_schema("request", sample_input, convert_to_provided_type=False)
@output_schema(sample_output_schema)
def run(request):
    df = pd.DataFrame(request)
    image = get_base64image_from_request(df)
    if image:
        prediction = get_prediction(image, session, signature)
        return AMLResponse(json.dumps([prediction]), 200)
    else:
        return AMLResponse("bad request", 400)

def get_base64image_from_request(request):
    image_string = request.iloc[0]["image"]
    image_bytes = base64.b64decode(image_string)
    image = Image.open(io.BytesIO(image_bytes))
    if image.mode != "RGB":
        image = image.convert("RGB")
    return image
