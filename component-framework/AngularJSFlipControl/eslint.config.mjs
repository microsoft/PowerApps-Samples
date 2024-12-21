import eslintjs from "@eslint/js";
import microsoftPowerApps from "@microsoft/eslint-plugin-power-apps";
import pluginPromise from "eslint-plugin-promise";
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
	{
		plugins: {
			"@microsoft/power-apps": microsoftPowerApps,
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

		rules: {
			"@typescript-eslint/no-unused-vars": "off",
			"@typescript-eslint/no-misused-promises": [
				"error",
				{
					checksVoidReturn: {
						arguments: false,
					},
				},
			],
			"@typescript-eslint/no-unsafe-member-access": "off",
		},
	},
];
