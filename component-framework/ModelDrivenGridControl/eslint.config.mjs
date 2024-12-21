import eslintjs from "@eslint/js";
import microsoftPowerApps from "@microsoft/eslint-plugin-power-apps";
import pluginPromise from "eslint-plugin-promise";
import reactPlugin from "eslint-plugin-react";
import reactHooksPlugin from "eslint-plugin-react-hooks";
import globals from "globals";
import typescriptEslint from "typescript-eslint";

/** @type {import('eslint').Linter.Config[]} */
export default [
	{
		ignores: ["**/generated"],
	},
	eslintjs.configs.recommended,
	...typescriptEslint.configs.recommendedTypeChecked,
	...typescriptEslint.configs.stylisticTypeChecked,
	pluginPromise.configs["flat/recommended"],
	microsoftPowerApps.configs.paCheckerHosted,
	reactPlugin.configs.flat.recommended,
	{
		plugins: {
			"@microsoft/power-apps": microsoftPowerApps,
			"react-hooks": reactHooksPlugin,
		},

		languageOptions: {
			globals: {
				...globals.browser,
				ComponentFramework: true,
			},
			parserOptions: {
				ecmaVersion: 2020,
				sourceType: "module",
				projectService: true,
				tsconfigRootDir: import.meta.dirname,
			},
		},

		settings: {
			react: {
				version: "detect",
			},
		},

		rules: {
			...reactHooksPlugin.configs.recommended.rules,
		},
	},
];
