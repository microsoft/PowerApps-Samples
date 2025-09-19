/**
 * Input required to call the Predict endpoint.
 */
interface RetrievePromptResponseInput {
	baseUrl: string;
	modelId: string;
	requestInputs?: Record<string, unknown>;
}

/**
 * File part returned from the prediction service.
 */
interface FileOutput {
	base64_content: string;
	content_type: string;
	file_name: string;
}

/**
 * Structured text output of the prediction.
 */
interface StructuredOutput {
	text?: string;
	mimetype?: string;
}

/**
 * Top-level prediction payload with either structured text or files.
 */
interface PredictionOutput {
	structuredOutput?: StructuredOutput;
	files?: FileOutput[];
}

/**
 * Wrapper for the v2 response format.
 */
interface ResponseV2 {
	predictionOutput?: PredictionOutput;
}

/**
 * Output of the Predict call, including optional error information.
 */
export interface RetrievePromptResponseOutput {
	responsev2?: ResponseV2;
	error?: {
		status?: number;
		message?: string;
	};
}

/**
 * Calls the Predict API for the given model and returns the parsed response or an error object.
 */
export async function retrievePromptResponse({
	baseUrl,
	modelId,
	requestInputs,
}: RetrievePromptResponseInput): Promise<RetrievePromptResponseOutput> {
	// Construct the endpoint URL
	const url = `${baseUrl}/api/data/v9.0/msdyn_aimodels(${modelId})/Microsoft.Dynamics.CRM.Predict`;

	const body = {
		version: "2.0",
		requestv2: {
			"@odata.type": "#Microsoft.Dynamics.CRM.expando",
			...requestInputs,
		},
	};

	const headers = {
		"Content-Type": "application/json",
	};

	try {
		const response = await fetch(url, {
			method: "POST",
			headers: headers,
			body: JSON.stringify(body),
		});

		if (!response.ok) {
			// Try to get error details from response body
			let errorMessage = `Call to prompt API failed with status: ${response.status}`;
			try {
				const errorBody = await response.text();
				if (errorBody) {
					errorMessage += ` - ${errorBody}`;
				}
			} catch {
				// If we can't read the error body, use the default message
			}

			return {
				responsev2: {},
				error: {
					status: response.status,
					message: errorMessage,
				},
			};
		}

		const data = (await response.json()) as RetrievePromptResponseOutput;
		return data;
	} catch (error) {
		console.error("Error fetching prompt API response:", error);

		// Check if it's a network or other fetch error
		const errorMessage = error instanceof Error ? error.message : String(error);

		return {
			responsev2: {},
			error: {
				status: 0, // Use 0 to indicate network/connection error
				message: `Network error: ${errorMessage}`,
			},
		};
	}
}
