import * as React from "react";
import { PrimaryButton, DefaultButton } from "@fluentui/react";

export interface MainContainerProps {
	width: number;
	height: number;
	onLoadData: () => void;
	onClearData: () => void;
}

export class MainContainer extends React.Component<MainContainerProps> {
	public render(): React.ReactNode {
		const { width, height, onLoadData, onClearData } = this.props;
		return (
			<>
				<PrimaryButton text="Load Data" style={{ width, height: height / 2 }} onClick={() => onLoadData()} />
				<DefaultButton text="Clear Data" style={{ width, height: height / 2 }} onClick={() => onClearData()} />
			</>
		);
	}
}
