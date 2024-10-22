/*
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License. See License.txt in the project root for license information.
 */

/* eslint-disable */

import * as React from 'react';
import Monaco from "@monaco-editor/react";
import { mergeStyleSets } from '@fluentui/react';

export interface IEditorProps {
    callback: (newvalue: string) => void;
    defaultValue: string;
	defaultLanguage: string;
}

const useStyles = mergeStyleSets({
	body: {
	  width: '100%',
	},
	container: {
	  border: 'solid 1px #ddd',
	  height: '400px',
	  width: '100%',
	
	  textAlign: 'left',
	},
	label: {
		textAlign: 'left',
	}
  });

self.MonacoEnvironment = {
	getWorkerUrl: function (_moduleId: any, label: string) {
		if (label === 'json') {
			return './json.worker.bundle.js';
		}
		if (label === 'css' || label === 'scss' || label === 'less') {
			return './css.worker.bundle.js';
		}
		if (label === 'html' || label === 'handlebars' || label === 'razor') {
			return './html.worker.bundle.js';
		}
		if (label === 'typescript' || label === 'javascript') {
			return './ts.worker.bundle.js';
		}
		return './editor.worker.bundle.js';
	}
};

export const Editor: React.FC<IEditorProps> = (props: IEditorProps) => {

	const handleEditorWillMount = (monaco: any) => {
		monaco.languages.register({ id: 'liquid' });

		// Register a tokens provider for the language
		monaco.languages.setMonarchTokensProvider('liquid', {
			// Set defaultToken to invalid to see what you do not tokenize yet
			defaultToken: '',
						tokenPostfix: '',
						// ignoreCase: true,
						keywords: [
							'assign', 'capture', 'endcapture', 'increment', 'decrement',
							'if', 'else', 'elsif', 'endif', 'for', 'endfor', 'break',
							'continue', 'limit', 'offset', 'range', 'reversed', 'cols',
							'case', 'endcase', 'when', 'block', 'endblock', 'true', 'false',
							'in', 'unless', 'endunless', 'cycle', 'tablerow', 'endtablerow',
							'contains', 'startswith', 'endswith', 'comment', 'endcomment',
							'raw', 'endraw', 'editable', 'endentitylist', 'endentityview', 'endinclude',
							'endmarker', 'entitylist', 'entityview', 'forloop', 'image', 'include',
							'marker', 'outputcache', 'plugin', 'style', 'text', 'widget',
							'abs', 'append', 'at_least', 'at_most', 'capitalize', 'ceil', 'compact',
							'concat', 'date', 'default', 'divided_by', 'downcase', 'escape',
							'escape_once', 'first', 'floor', 'join', 'last', 'lstrip', 'map',
							'minus', 'modulo', 'newline_to_br', 'plus', 'prepend', 'remove',
							'remove_first', 'replace', 'replace_first', 'reverse', 'round',
							'rstrip', 'size', 'slice', 'sort', 'sort_natural', 'split', 'strip',
							'strip_html', 'strip_newlines', 'times', 'truncate', 'truncatewords',
							'uniq', 'upcase', 'url_decode', 'url_encode'
						],
						operators: [
						'==', '>', '<', '<=', '>=', '!=', 'and', 'or'
						],
						symbols: /[~!@#%\^&*-+=|\\:`<>.?\/]+/,
						exponent: /[eE][\-+]?[0-9]+/,
						// The main tokenizer for our languages
						tokenizer: {
							root: [
							// identifiers and keywords
							[/([a-zA-Z_\$][\w\$]*)(\s*)/, { cases: { '@default': 'identifier' } }],
							[/<!--/, 'comment.html', '@comment'],
							[/\{\%\s*comment\s*\%\}/, 'comment.html', '@comment'],
							[/\{\{|\{\%/, { token: '@rematch', switchTo: '@liquidInSimpleState.root' }],
							[/(<)(\w+)(\/>)/, ['delimiter.html', 'tag.html', 'delimiter.html']],
							[/(<)(script)/, ['delimiter.html', { token: 'tag.html', next: '@script' }]],
							[/(<)(style)/, ['delimiter.html', { token: 'tag.html', next: '@style' }]],
							[/(<)([:\w]+)/, ['delimiter.html', { token: 'tag.html', next: '@liquidInHtmlAttribute' }]],
							[/(<\/)(\w+)/, ['delimiter.html', { token: 'tag.html', next: '@liquidInHtmlAttribute' }]],
							[/</, 'delimiter.html'],
							[/\{/, 'delimiter.html'],
							{ include: 'numbers' },
							[/[ \t\r\n]+/], // whitespace
							[/[^<{]+/], // text
							],
							// After <script
							script: [
								[/\{\{\-?|\{\%\-?/, { token: '@rematch', switchTo: '@liquidInSimpleState.script' }],
								[/type/, 'attribute.name', '@scriptAfterType'],
								[/"([^"]*)"/, 'attribute.value'],
								[/'([^']*)'/, 'attribute.value'],
								[/[\w\-]+/, 'attribute.name'],
								[/=/, 'delimiter'],
								[/>/, { token: 'delimiter.html', next: '@scriptEmbedded.text/javascript', nextEmbedded: 'text/javascript' }],
								[/[ \t\r\n]+/], // whitespace
								[/(<\/)(script\s*)(>)/, ['delimiter.html', 'tag.html', { token: 'delimiter.html', next: '@pop' }]]
							],

							// After <script ... type
							scriptAfterType: [
								[/\{\{\-?|\{\%\-?/, { token: '@rematch', switchTo: '@liquidInSimpleState.scriptAfterType' }],
								[/=/, 'delimiter', '@scriptAfterTypeEquals'],
								[/>/, { token: 'delimiter.html', next: '@scriptEmbedded.text/javascript', nextEmbedded: 'text/javascript' }], // cover invalid e.g. <script type>
								[/[ \t\r\n]+/], // whitespace
								[/<\/script\s*>/, { token: '@rematch', next: '@pop' }]
							],

							// After <script ... type =
							scriptAfterTypeEquals: [
								[/\{\{\-?|\{\%\-?/, { token: '@rematch', switchTo: '@liquidInSimpleState.scriptAfterTypeEquals' }],
								[/"([^"]*)"/, { token: 'attribute.value', switchTo: '@scriptWithCustomType.$1' }],
								[/'([^']*)'/, { token: 'attribute.value', switchTo: '@scriptWithCustomType.$1' }],
								[/>/, { token: 'delimiter.html', next: '@scriptEmbedded.text/javascript', nextEmbedded: 'text/javascript' }], // cover invalid e.g. <script type=>
								[/[ \t\r\n]+/], // whitespace
								[/<\/script\s*>/, { token: '@rematch', next: '@pop' }]
							],

							// After <script ... type = $S2
							scriptWithCustomType: [
								[/\{\{\-?|\{\%\-?/, { token: '@rematch', switchTo: '@liquidInSimpleState.scriptWithCustomType.$S2' }],
								[/>/, { token: 'delimiter.html', next: '@scriptEmbedded.$S2', nextEmbedded: '$S2' }],
								[/"([^"]*)"/, 'attribute.value'],
								[/'([^']*)'/, 'attribute.value'],
								[/[\w\-]+/, 'attribute.name'],
								[/=/, 'delimiter'],
								[/[ \t\r\n]+/], // whitespace
								[/<\/script\s*>/, { token: '@rematch', next: '@pop' }]
							],

							scriptEmbedded: [
								[/\{\{\-?|\{\%\-?/, { token: '@rematch', switchTo: '@liquidInEmbeddedState.scriptEmbedded.$S2', nextEmbedded: '@pop' }],
								[/<\/script/, { token: '@rematch', next: '@pop', nextEmbedded: '@pop' }]
							],

							// -- END <script> tags handling
							// -- BEGIN <style> tags handling

							// After <style
							style: [
								[/\{\{\-?|\{\%\-?/, { token: '@rematch', switchTo: '@liquidInSimpleState.style' }],
								[/type/, 'attribute.name', '@styleAfterType'],
								[/"([^"]*)"/, 'attribute.value'],
								[/'([^']*)'/, 'attribute.value'],
								[/[\w\-]+/, 'attribute.name'],
								[/=/, 'delimiter'],
								[/>/, { token: 'delimiter.html', next: '@styleEmbedded.text/css', nextEmbedded: 'text/css' }],
								[/[ \t\r\n]+/], // whitespace
								[/(<\/)(style\s*)(>)/, ['delimiter.html', 'tag.html', { token: 'delimiter.html', next: '@pop' }]]
							],

							// After <style ... type
							styleAfterType: [
								[/\{\{\-?|\{\%\-?/, { token: '@rematch', switchTo: '@liquidInSimpleState.styleAfterType' }],
								[/=/, 'delimiter', '@styleAfterTypeEquals'],
								[/>/, { token: 'delimiter.html', next: '@styleEmbedded.text/css', nextEmbedded: 'text/css' }], // cover invalid e.g. <style type>
								[/[ \t\r\n]+/], // whitespace
								[/<\/style\s*>/, { token: '@rematch', next: '@pop' }]
							],

							// After <style ... type =
							styleAfterTypeEquals: [
								[/\{\{\-?|\{\%\-?/, { token: '@rematch', switchTo: '@liquidInSimpleState.styleAfterTypeEquals' }],
								[/"([^"]*)"/, { token: 'attribute.value', switchTo: '@styleWithCustomType.$1' }],
								[/'([^']*)'/, { token: 'attribute.value', switchTo: '@styleWithCustomType.$1' }],
								[/>/, { token: 'delimiter.html', next: '@styleEmbedded.text/css', nextEmbedded: 'text/css' }], // cover invalid e.g. <style type=>
								[/[ \t\r\n]+/], // whitespace
								[/<\/style\s*>/, { token: '@rematch', next: '@pop' }]
							],

							// After <style ... type = $S2
							styleWithCustomType: [
								[/\{\{\-?|\{\%\-?/, { token: '@rematch', switchTo: '@liquidInSimpleState.styleWithCustomType.$S2' }],
								[/>/, { token: 'delimiter.html', next: '@styleEmbedded.$S2', nextEmbedded: '$S2' }],
								[/"([^"]*)"/, 'attribute.value'],
								[/'([^']*)'/, 'attribute.value'],
								[/[\w\-]+/, 'attribute.name'],
								[/=/, 'delimiter'],
								[/[ \t\r\n]+/], // whitespace
								[/<\/style\s*>/, { token: '@rematch', next: '@pop' }]
							],

							styleEmbedded: [
								[/\{\{\-?|\{\%\-?/, { token: '@rematch', switchTo: '@liquidInEmbeddedState.styleEmbedded.$S2', nextEmbedded: '@pop' }],
								[/<\/style/, { token: '@rematch', next: '@pop', nextEmbedded: '@pop' }]
							],

							// -- END <style> tags handling

							comment: [
							[/\{\%\s*endcomment\s*\%\}/, 'comment.hmtl', '@pop'],
							[/-->/, 'comment.html', '@pop'],
							[/\b(?!-->|endcomment\s*\%\}\b)\w+/, 'comment.content.html'],
							[/./, 'comment.content.html']
							],
							// Support inside html tags
							liquidInHtmlAttribute: [
									[/"\{\{\-?|"\{\%\-?/, { token: 'delimiter.liquid', next: '@liquidInHtmlAttributeEmbedded' }],
									[/\/?>/, 'delimiter.html', '@pop'],
									[/"([^"]*)"/, 'attribute.value'],
									[/'([^']*)'/, 'attribute.value'],
									[/[\w\-]+/, 'attribute.name'],
									[/=/, 'delimiter'],
									[/[ \t\r\n]+/], // whitespace
							],
							liquidInHtmlAttributeEmbedded: [
								[/\'(.*)\'/],
								[/\-?\%\}"|\-?\}\}"/, 'delimiter.liquid', '@pop'],
								[/([a-zA-Z_\$][\w\$]*)/, {
									cases: {
										'@keywords': 'keyword',
										'@operators': 'operator',
										'@default': 'variable.parameter.liquid'
									}
								}],
							],
							liquidInSimpleState: [
								[/\{\{\-?|\{\%\-?/, 'delimiter.liquid'],
								[/\-?\}\}|\-?\%\}/, { token: 'delimiter.liquid', switchTo: '@$S2.$S3' }],
								{ include: 'liquidRoot' }
							],
							liquidInEmbeddedState: [
								[/\{\{\-?|\{\%\-?/, 'delimiter.liquid'],
								[/\-?\}\}|\-?\%\}/, { token: 'delimiter.liquid', switchTo: '@$S2.$S3', nextEmbedded: '$S3' }],
								{ include: 'liquidRoot' }
							],
							liquidRoot: [
								[/\'(.*?)\'|\"(.*?)\"/],
								[/([a-zA-Z_\$][\w\$]*)/, {
									cases: {
										'@keywords': 'keyword',
										'@operators': 'operator',
										'@default': 'variable.parameter.liquid'
									}
								}],
								{ include: 'numbers' },
								[/@symbols/, {
									cases: {
										'@operators': 'operator',
										'@default': ''
									}
								}],
							],
							numbers: [
								// numbers
							[/\d+\.\d*(@exponent)?/, 'number.float'],
							[/\.\d+(@exponent)?/, 'number.float'],
							[/\d+@exponent/, 'number.float'],
							[/\d+/, 'number'],
							[/[;,.]/, 'delimiter'],
							]
						},
					});

		monaco.languages.setLanguageConfiguration('liquid', {
			wordPattern: /(-?\d*\.\d\w*)|([^\`\~\!\@\$\^\&\*\(\)\=\+\[\{\]\}\\\|\;\:\'\"\,\.\<\>\/\s]+)/g,

			brackets: [
				['{{-', '-}}'],
				['{{', '}}'],
				['{%-', '-%}'],
				['{%', '%}'],
				['{', '}'],
				['[', ']'],
				['(', ')'],
			],
			autoClosingPairs: [
				{ open: '{%-', close: '-%}' },
				{ open: '{%', close: '%}' },
				{ open: '{', close: '}' },
				{ open: '[', close: ']' },
				{ open: '(', close: ')' },
				{ open: '"', close: '"' },
				{ open: '\'', close: '\'' }
			],

			surroundingPairs: [
				{ open: '<', close: '>' },
				{ open: '"', close: '"' },
				{ open: '\'', close: '\'' }
			]
		});

		monaco.languages.registerCompletionItemProvider('liquid', {
			provideCompletionItems: function () {
				var autocompleteProviderItems = [];
				var keywords = ['assign', 'capture', 'endcapture', 'increment', 'decrement',
							'if', 'else', 'elsif', 'endif', 'for', 'endfor', 'break',
							'continue', 'limit', 'offset', 'range', 'reversed', 'cols',
							'case', 'endcase', 'when', 'block', 'endblock', 'true', 'false',
							'in', 'unless', 'endunless', 'cycle', 'tablerow', 'endtablerow',
							'contains', 'startswith', 'endswith', 'comment', 'endcomment',
							'raw', 'endraw', 'editable', 'endentitylist', 'endentityview', 'endinclude',
							'endmarker', 'entitylist', 'entityview', 'forloop', 'image', 'include',
							'marker', 'outputcache', 'plugin', 'style', 'text', 'widget',
							'abs', 'append', 'at_least', 'at_most', 'capitalize', 'ceil', 'compact',
							'concat', 'date', 'default', 'divided_by', 'downcase', 'escape',
							'escape_once', 'first', 'floor', 'join', 'last', 'lstrip', 'map',
							'minus', 'modulo', 'newline_to_br', 'plus', 'prepend', 'remove',
							'remove_first', 'replace', 'replace_first', 'reverse', 'round',
							'rstrip', 'size', 'slice', 'sort', 'sort_natural', 'split', 'strip',
							'strip_html', 'strip_newlines', 'times', 'truncate', 'truncatewords',
							'uniq', 'upcase', 'url_decode', 'url_encode'];

				for (var i = 0; i < keywords.length; i++) {
					autocompleteProviderItems.push({ 'label': keywords[i], kind: monaco.languages.CompletionItemKind.Keyword });
				}

				return autocompleteProviderItems;
			}
		});
	}

	return <Monaco
				className={useStyles.container}
				defaultValue={props.defaultValue}
				defaultLanguage={props.defaultLanguage}
				beforeMount={handleEditorWillMount}
				onChange={(newValue) => props.callback(newValue!)}
			/>;
};