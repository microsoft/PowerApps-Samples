import * as React from "react";
import {
	setIconOptions,
	PersonaSize,
	IFacepileProps,
	OverflowButtonType,
	IFacepilePersona,
	IDropdownOption,
	Facepile,
	Checkbox,
	Dropdown,
	Slider,
} from "@fluentui/react";
import { facepilePersonas } from "./FacepileExampleData";

// Suppress office UI fabric icon warnings.
setIconOptions({
	disableWarnings: true,
});

export interface IFacepileBasicExampleProps {
	numberOfFaces?: number;
	numberFacesChanged?: (newValue: number) => void;
}

export interface IFacepileBasicExampleState extends React.ComponentState, IFacepileBasicExampleProps {
	personaSize: PersonaSize;
	imagesFadeIn: boolean;
}

export class FacepileBasicExample extends React.Component<IFacepileBasicExampleProps, IFacepileBasicExampleState> {
	constructor(props: IFacepileBasicExampleProps) {
		super(props);

		this.state = {
			// eslint-disable-next-line @typescript-eslint/prefer-nullish-coalescing
			numberOfFaces: props.numberOfFaces || 3,
			imagesFadeIn: true,
			personaSize: PersonaSize.size32,
		};
	}

	public render(): JSX.Element {
		const { numberOfFaces, personaSize } = this.state;
		const facepileProps: IFacepileProps = {
			personaSize,
			personas: facepilePersonas,
			overflowButtonType: OverflowButtonType.descriptive,
			maxDisplayablePersonas: this.state.numberOfFaces,
			getPersonaProps: (persona: IFacepilePersona) => ({
				imageShouldFadeIn: this.state.imagesFadeIn,
			}),
			ariaDescription: "To move through the items use left and right arrow keys.",
		};

		return (
			<div className={"msFacepileExample"}>
				<Facepile {...facepileProps} />
				<div className={"control"}>
					<Slider
						label="Number of Personas:"
						min={1}
						max={5}
						step={1}
						showValue={true}
						value={numberOfFaces}
						onChange={this.onChangePersonaNumber}
					/>
					<Dropdown
						label="Persona Size:"
						selectedKey={this.state.personaSize}
						options={[
							{ key: PersonaSize.size16, text: "16px" },
							{ key: PersonaSize.size24, text: "24px" },
							{ key: PersonaSize.size28, text: "28px" },
							{ key: PersonaSize.size32, text: "32px" },
							{ key: PersonaSize.size40, text: "40px" },
						]}
						onChange={this.onChangePersonaSize}
					/>
					<Checkbox
						className={"exampleCheckbox"}
						label="Fade In"
						checked={this.state.imagesFadeIn}
						onChange={this.onChangeFadeIn}
					/>
				</div>
			</div>
		);
	}

	private onChangeFadeIn = (
		ev: React.FormEvent<HTMLElement | HTMLInputElement> | undefined,
		checked?: boolean
	): void => {
		this.setState((prevState: IFacepileBasicExampleState): IFacepileBasicExampleState => {
			prevState.imagesFadeIn = !!checked;
			return prevState;
		});
	};

	private onChangePersonaNumber = (value: number): void => {
		this.setState((prevState: IFacepileBasicExampleState): IFacepileBasicExampleState => {
			prevState.numberOfFaces = value;
			return prevState;
		});
		if (this.props.numberFacesChanged) {
			this.props.numberFacesChanged(value);
		}
	};

	private onChangePersonaSize = (event: React.FormEvent<HTMLDivElement>, value?: IDropdownOption): void => {
		this.setState((prevState: IFacepileBasicExampleState): IFacepileBasicExampleState => {
			prevState.personaSize = value ? (value.key as PersonaSize) : PersonaSize.size32;
			return prevState;
		});
	};
}
