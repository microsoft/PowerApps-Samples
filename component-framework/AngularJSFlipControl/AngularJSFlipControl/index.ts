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

/**
 * This JS shows how to use a 3rd party library to create a control in PowerApps Control Framework
 * Sample is based on angular.js, angular-ui, angular-animate, angular-sanitize, bootstrap for sampling purpose only
 * The code may not reveal the best practices for the mentioned 3rd party libraries
 */
import { IInputs, IOutputs } from "./generated/ManifestTypes";
import * as angular from "angular";
import * as ngAnimate from "angular-animate";
import * as ngSanitize from "angular-sanitize";
import * as uiBootstrap from "angular-ui-bootstrap";

export class AngularJSFlipControl implements ComponentFramework.StandardControl<IInputs, IOutputs> {
	// Element id of the ng-app div. Type: string
	private _appDivId: string;

	// ng-app app id. Type: string
	private _appId: string;

	// ng-controller. Type: string
	private _controllerId: string;

	// PCF framework delegate which will be assigned to this object which would be called whenever any update happens. Type: function
	private _notifyOutputChanged: () => void;

	// Model of the bind field. Type: Boolean
	private _currentValue: boolean;

	// Option Label Text when Option is True. The Text is from attribute customization. Type: string
	private _optionTrueLabel: string;

	// Option Label Text when Option is False. The Text is from attribute customization. Type: string
	private _optionFalseLabel: string;

	/**
	 * Empty constructor.
	 */
	constructor() {
		// no-op: method not leveraged by this example custom control
	}

	/**
	 * Used to initialize the control instance. Controls can kick off remote server calls and other initialization actions here.
	 * Data-set values are not initialized here, use updateView.
	 * @param context The entire property bag available to control via Context Object; It contains values as set up by the customizer mapped to property names defined in the manifest, as well as utility functions.
	 * @param notifyOutputChanged A callback method to alert the framework that the control has new outputs ready to be retrieved asynchronously.
	 * @param state A piece of data that persists in one session for a single user. Can be set at any point in a controls life cycle by calling 'setControlState' in the Mode interface.
	 * @param container If a control is marked control-type='standard', it will receive an empty div element within which it can render its content.
	 */
	public init(
		context: ComponentFramework.Context<IInputs>,
		notifyOutputChanged: () => void,
		state: ComponentFramework.Dictionary,
		container: HTMLDivElement
	): void {
		// We need a random integer from 1-100, so that for a form of multiple fields bind to same attribute, we could differentiate
		const randomInt: number = Math.floor(Math.floor(100) * Math.random());

		this._appDivId = this.createUniqueId(context, "angularflip_controlid", randomInt);
		this._appId = this.createUniqueId(context, "AngularJSFlipControl", randomInt);
		this._controllerId = this.createUniqueId(context, "powerApps.angularui.demo", randomInt);
		this._notifyOutputChanged = notifyOutputChanged;

		// Assign Model the value of the bind attribute
		this._currentValue = context.parameters.flipModel.raw;

		// Initialize the True/False Label texts from the attribute metadata
		this.initializeOptionsLabel(context);

		// Create HTML structure for the control
		const appDiv: HTMLDivElement = document.createElement("div");
		appDiv.setAttribute("id", this._appDivId);
		appDiv.setAttribute("ng-controller", this._appId);
		appDiv.setAttribute("ng-app", this._controllerId);

		// Below sample html are from Angular-UI single toggle sample code
		// https://angular-ui.github.io/bootstrap/
		appDiv.innerHTML =
			"<pre>{{labelModel}}</pre><button type='button' class='btn btn-primary' ng-model='flipButtonModel' uib-btn-checkbox btn-checkbox-true='1' btn-checkbox-false='0'>Flip</button>";

		// Container appends the HTML structure
		container.appendChild(appDiv);

		// Angular code. Angular module/controller initialization.
		angular.module(this._controllerId, [ngAnimate, ngSanitize, uiBootstrap]);
		angular.module(this._controllerId).controller(this._appId, ($scope) => {
			// Intialize 'labelModel'. Assign initial option text to the Angular $scope labelModel. It will be revealed in '<pre>{{labelModel}}</pre>'
			$scope.labelModel = this._currentValue ? this._optionTrueLabel : this._optionFalseLabel;

			// Intialize 'flipButtonModel'. Assign bind attribute value to Angular $scope flipButtonModel. The Flip button also bind to this 'flipButtonModel', so when we click, it will flip
			$scope.flipButtonModel = this._currentValue ? 0 : 1;

			// Watch the click of the flip button
			// eslint-disable-next-line @typescript-eslint/no-unsafe-call
			$scope.$watchCollection("flipButtonModel", () => {
				// Update the label text when Flip Button clicks
				if ($scope.flipButtonModel) {
					$scope.labelModel = this._optionFalseLabel;
				} else {
					$scope.labelModel = this._optionTrueLabel;
				}

				// Call updateOutputIfNeeded and inform PCF framework that bind attribute value need update
				this.updateOutputIfNeeded(!$scope.flipButtonModel);
			});
		});

		// Angular code. Create an App based on the new appDivId
		angular.element(document).ready(() => {
			const divId = document.getElementById(this._appDivId);
			if (divId) {
				angular.bootstrap(divId, [this._controllerId]);
			}
		});
	}

	/**
	 * Get UniqueId so as to avoid id conflict between multiple fields bind to same attribute
	 * @param context The "Input Properties" containing the parameters, control metadata and interface functions.
	 * @param passInString input string as suffix
	 * @param randomInt random integer
	 * @returns a string of uniqueId includes attribute logicalname + passIn specialized string + random Integer
	 */
	private createUniqueId(
		context: ComponentFramework.Context<IInputs>,
		passInString: string,
		randomInt: number
	): string {
		return `${context.parameters?.flipModel.attributes?.LogicalName}-${passInString}${randomInt}`;
	}

	/**
	 * Initialize Options Label to use the attribute label from Metadata
	 * @param context The "Input Properties" containing the parameters, control metadata and interface functions.
	 */
	private initializeOptionsLabel(context: ComponentFramework.Context<IInputs>): void {
		// Get option label texts from metadata
		const optionsMetadata = context.parameters.flipModel.attributes?.Options;
		optionsMetadata?.forEach((option: ComponentFramework.PropertyHelper.OptionMetadata) => {
			if (option.Value) {
				this._optionFalseLabel = option.Label;
			} else {
				this._optionTrueLabel = option.Label;
			}
		});
	}

	/**
	 * Update Angular 'flipButtonModel' if needed
	 * @param newValue new value
	 */
	private updateFlipButtonModelIfNeeded(newValue: boolean): void {
		if ((newValue && !this._currentValue) || (!newValue && this._currentValue)) {
			this._currentValue = newValue;

			// Angular Code. Update the 'flipButtonModel' value
			const $scope = angular.element(document.getElementById(this._appDivId)!).scope();

			// eslint-disable-next-line @typescript-eslint/no-explicit-any
			$scope.$apply(($scope: any) => {
				// 'flipButtonModel' value is either 1 or 0
				$scope.flipButtonModel = newValue ? 0 : 1;
			});
		}
	}

	/**
	 * Update value in Power Control Framework
	 * @param newValue new value
	 */
	private updateOutputIfNeeded(newValue: boolean): void {
		if (newValue !== this._currentValue) {
			this._currentValue = newValue;
			this._notifyOutputChanged();
		}
	}

	/**
	 * Called when any value in the property bag has changed. This includes field values, data-sets, global values such as container height and width, offline status, control metadata values such as label, visible, etc.
	 * @param context The entire property bag available to control via Context Object; It contains values as set up by the customizer mapped to names defined in the manifest, as well as utility functions
	 */
	public updateView(context: ComponentFramework.Context<IInputs>): void {
		// An attribute value from Control Framework could be updated even after init cycle, clientAPI, post Save response can update the attribute value and the Flip control should reveal the new value.
		this.updateFlipButtonModelIfNeeded(context.parameters.flipModel.raw);
	}

	/**
	 * It is called by the framework prior to a control receiving new data.
	 * @returns an object based on nomenclature defined in manifest, expecting object[s] for property marked as "bound" or "output"
	 */
	public getOutputs(): IOutputs {
		const returnValue = this._currentValue;
		return { flipModel: returnValue };
	}

	/**
	 * Called when the control is to be removed from the DOM tree. Controls should use this call for cleanup.
	 * i.e. cancelling any pending remote calls, removing listeners, etc.
	 */
	public destroy(): void {
		// no-op: method not leveraged by this example custom control
	}
}
