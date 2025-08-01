interface RetrievePromptResponseInput {
    baseUrl: string;
    modelId: string;
    requestInputs?: Record<string, unknown>;
}

interface FileOutput {
    base64_content: string;
    content_type: string;
    file_name: string;
}

interface StructuredOutput {
    text?: string;
    mimetype?: string;
}

interface PredictionOutput {
    structuredOutput?: StructuredOutput;
    files?: FileOutput[];
}

interface ResponseV2 {
    predictionOutput?: PredictionOutput;
}

export interface RetrievePromptResponseOutput {
    responsev2?: ResponseV2;
    error?: {
        status?: number;
        message?: string;
    };
}

export async function retrievePromptResponse(
    { baseUrl, modelId, requestInputs  }: RetrievePromptResponseInput,
): Promise<RetrievePromptResponseOutput> {
    // Construct the endpoint URL
    const url = `${baseUrl}/api/data/v9.0/msdyn_aimodels(${modelId})/Microsoft.Dynamics.CRM.Predict`;

    const body = {
        version: "2.0",
        requestv2: {
        "@odata.type": "#Microsoft.Dynamics.CRM.expando",
        ...requestInputs
        }
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
                    message: errorMessage
                }
            };
        }

        const data = await response.json() as RetrievePromptResponseOutput;
        return data;
    } catch (error) {
        console.error("Error fetching prompt API response:", error);

        // Check if it's a network or other fetch error
        const errorMessage = error instanceof Error ? error.message : String(error);
        
        return { 
            responsev2: {}, 
            error: {
                status: 0, // Use 0 to indicate network/connection error
                message: `Network error: ${errorMessage}`
            }
        };
    }
}