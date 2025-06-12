/*
	This file is part of the Microsoft PowerApps code samples.
	Copyright (C) Microsoft Corporation.  All rights reserved.
	This source code is intended only as a supplement to Microsoft Development Tools and/or
	on-line documentation.  See these other materials for detailed information regarding
	Microsoft code samples.

	THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER
	EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF
	MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
 */

import { IInputs, IOutputs } from "./generated/ManifestTypes";

export class TeamsClipboardControl implements ComponentFramework.StandardControl<IInputs, IOutputs> {
	// Control container
	private _container: HTMLDivElement;
	
	// Main container for the control
	private _mainContainer: HTMLDivElement;
	
	// Text display element
	private _textElement: HTMLDivElement;
	
	// Copy button
	private _copyButton: HTMLButtonElement;
	
	// Feedback element
	private _feedbackElement: HTMLDivElement;
	
	// Fallback container
	private _fallbackContainer: HTMLDivElement;
	
	// Hidden input for fallback copying
	private _hiddenInput: HTMLInputElement;
	
	// Current text to copy
	private _currentText = "";
	
	// Flag to track if Teams SDK is available
	private _teamsSDKAvailable = false;

	/**
	 * Used to initialize the control instance.
	 */
	public init(
		context: ComponentFramework.Context<IInputs>,
		notifyOutputChanged: () => void,
		state: ComponentFramework.Dictionary,
		container: HTMLDivElement
	): void {
		this._container = container;
		this.createControlElements();
		this.checkTeamsSDKAvailability();
	}

	/**
	 * Called when any value in the property bag has changed.
	 */
	public updateView(context: ComponentFramework.Context<IInputs>): void {
		const textToCopy = context.parameters.textToCopy.raw ?? "";
		const buttonLabel = context.parameters.buttonLabel.raw ?? "Copy";
		
		this._currentText = textToCopy;
		this._textElement.textContent = textToCopy;
		this._copyButton.textContent = buttonLabel;
		
		// Hide feedback when text changes
		this.hideFeedback();
	}

	/**
	 * Creates the HTML elements for the control
	 */
	private createControlElements(): void {
		// Main container
		this._mainContainer = document.createElement("div");
		this._mainContainer.className = "teams-clipboard-container";
		
		// Text display
		this._textElement = document.createElement("div");
		this._textElement.className = "teams-clipboard-text";
		
		// Copy button
		this._copyButton = document.createElement("button");
		this._copyButton.className = "teams-clipboard-button";
		this._copyButton.textContent = "Copy";
		this._copyButton.addEventListener("click", this.handleCopyClick.bind(this));
		
		// Feedback element
		this._feedbackElement = document.createElement("div");
		this._feedbackElement.className = "teams-clipboard-feedback hidden";
		
		// Hidden input for fallback
		this._hiddenInput = document.createElement("input");
		this._hiddenInput.className = "teams-clipboard-hidden-input";
		this._hiddenInput.type = "text";
		
		// Fallback container
		this._fallbackContainer = document.createElement("div");
		this._fallbackContainer.className = "teams-clipboard-fallback";
		this._fallbackContainer.style.display = "none";
		
		// Assemble the control
		this._mainContainer.appendChild(this._textElement);
		this._mainContainer.appendChild(this._copyButton);
		this._mainContainer.appendChild(this._feedbackElement);
		this._mainContainer.appendChild(this._hiddenInput);
		
		this._container.appendChild(this._mainContainer);
		this._container.appendChild(this._fallbackContainer);
	}

	/**
	 * Handle copy button click
	 */
	private handleCopyClick(): void {
		// eslint-disable-next-line @typescript-eslint/no-floating-promises
		this.attemptCopy();
	}
	/**
	 * Check if Microsoft Teams SDK is available
	 */
	private checkTeamsSDKAvailability(): void {
		// Check if we're in a Teams environment
		// @ts-expect-error - Teams SDK may not be in type definitions
		if (typeof window !== "undefined" && window.microsoftTeams) {
			this._teamsSDKAvailable = true;
		}
	}

	/**
	 * Attempts to copy text using multiple strategies
	 */
	private async attemptCopy(): Promise<void> {
		if (!this._currentText) {
			this.showFeedback("No text to copy", false);
			return;
		}

		this._copyButton.disabled = true;
		this._copyButton.textContent = "Copying...";

		try {
			// Strategy 1: Modern Clipboard API
			if (await this.tryClipboardAPI()) {
				this.showFeedback("Copied successfully!", true);
				return;
			}

			// Strategy 2: Teams SDK (if available)
			if (await this.tryTeamsSDK()) {
				this.showFeedback("Copied via Teams!", true);
				return;
			}

			// Strategy 3: execCommand with hidden input
			if (this.tryExecCommand()) {
				this.showFeedback("Copied to clipboard!", true);
				return;
			}

			// Strategy 4: Selection-based copying
			if (this.trySelectionCopy()) {
				this.showFeedback("Copied via selection!", true);
				return;
			}

			// Strategy 5: Show fallback UI
			this.showFallbackUI();
			this.showFeedback("Manual copy required", false);

		} catch (error) {
			console.error("Copy failed:", error);
			this.showFallbackUI();
			this.showFeedback("Copy failed - see manual option", false);
		} finally {
			this._copyButton.disabled = false;
			this._copyButton.textContent = "Copy";
		}
	}

	/**
	 * Strategy 1: Try modern Clipboard API
	 */
	private async tryClipboardAPI(): Promise<boolean> {
		try {
			if (navigator.clipboard?.writeText) {
				await navigator.clipboard.writeText(this._currentText);
				return true;
			}
		} catch (error) {
			console.log("Clipboard API failed:", error);
		}
		return false;
	}

	/**
	 * Strategy 2: Try Microsoft Teams SDK
	 */
	private async tryTeamsSDK(): Promise<boolean> {
		try {
			if (this._teamsSDKAvailable) {
				// @ts-expect-error - Teams SDK may not be in type definitions
				// eslint-disable-next-line @typescript-eslint/no-unsafe-assignment
				const teams = window.microsoftTeams;
				// eslint-disable-next-line @typescript-eslint/no-unsafe-member-access
				if (teams?.clipboard?.writeText) {
					// eslint-disable-next-line @typescript-eslint/no-unsafe-call, @typescript-eslint/no-unsafe-member-access
					await teams.clipboard.writeText(this._currentText);
					return true;
				}
			}
		} catch (error) {
			console.log("Teams SDK failed:", error);
		}
		return false;
	}

	/**
	 * Strategy 3: Try execCommand with hidden input
	 */
	private tryExecCommand(): boolean {
		try {
			this._hiddenInput.value = this._currentText;
			this._hiddenInput.select();
			this._hiddenInput.setSelectionRange(0, 99999); // For mobile devices
			
			const success = document.execCommand('copy');
			if (success) {
				return true;
			}
		} catch (error) {
			console.log("execCommand failed:", error);
		}
		return false;
	}

	/**
	 * Strategy 4: Try selection-based copying
	 */
	private trySelectionCopy(): boolean {
		try {
			const range = document.createRange();
			range.selectNode(this._textElement);
			const selection = window.getSelection();
			if (selection) {
				selection.removeAllRanges();
				selection.addRange(range);
				
				const success = document.execCommand('copy');
				selection.removeAllRanges();
				
				if (success) {
					return true;
				}
			}
		} catch (error) {
			console.log("Selection copy failed:", error);
		}
		return false;
	}

	/**
	 * Show fallback UI for manual copying
	 */
	private showFallbackUI(): void {
		this._fallbackContainer.innerHTML = `
			<strong>Manual Copy Required:</strong><br>
			Please select and copy the text below manually (Ctrl+C or Cmd+C):
			<div class="teams-clipboard-fallback-text">${this._currentText}</div>
		`;
		this._fallbackContainer.style.display = "block";
	}

	/**
	 * Show feedback to the user
	 */
	private showFeedback(message: string, isSuccess: boolean): void {
		this._feedbackElement.textContent = message;
		this._feedbackElement.className = `teams-clipboard-feedback ${isSuccess ? 'success' : 'error'}`;
		
		// Auto-hide after 3 seconds
		setTimeout(() => {
			this.hideFeedback();
		}, 3000);
	}

	/**
	 * Hide feedback element
	 */
	private hideFeedback(): void {
		this._feedbackElement.className = "teams-clipboard-feedback hidden";
		this._fallbackContainer.style.display = "none";
	}

	/**
	 * It is called by the framework prior to a control receiving new data.
	 */
	public getOutputs(): IOutputs {
		return {};
	}

	/**
	 * Called when the control is to be removed from the DOM tree.
	 */
	public destroy(): void {
		// Cleanup event listeners
		if (this._copyButton) {
			this._copyButton.removeEventListener("click", this.handleCopyClick.bind(this));
		}
	}
}