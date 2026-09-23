import assert from "node:assert/strict";
import { readFileSync } from "node:fs";
import { createRequire } from "node:module";
import { fileURLToPath } from "node:url";
import { runInThisContext } from "node:vm";
import test from "node:test";
import ts from "typescript";

// Load the actual PCF handler without requiring a Dataverse host. Type checking
// and browser bundling remain the responsibility of the normal PCF build.
const sourceUrl = new URL("../CodeInterpreterControl/filePreviewHandler.ts", import.meta.url);
const sourcePath = fileURLToPath(sourceUrl);
const compiled = ts.transpileModule(readFileSync(sourceUrl, "utf8"), {
    compilerOptions: { module: ts.ModuleKind.CommonJS, target: ts.ScriptTarget.ES2020 },
}).outputText;
const compiledModule = { exports: {} };
runInThisContext(`(function (require, module, exports) {${compiled}\n})`, { filename: sourcePath })(
    createRequire(sourceUrl), compiledModule, compiledModule.exports,
);
const { FilePreviewHandler } = compiledModule.exports;
const labels = {
    Label_Sheet: "Sheet:",
    Label_Range: "Range:",
    Label_Cells: "Cells:",
    Label_Available_Sheets: "Available sheets:",
    Preview_Excel_Failed: "Excel preview failed: {0}",
};
const handler = new FilePreviewHandler({ resources: { getString: key => labels[key] ?? key } });
const xlsxMime = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

async function preview(fixture, mime = xlsxMime) {
    const bytes = readFileSync(new URL(`fixtures/${fixture}`, import.meta.url));
    const result = await handler.generatePreview(bytes.toString("base64"), mime, fixture);
    try {
        assert.equal(result.fileName, fixture);
        const download = await fetch(result.downloadUrl);
        assert.equal(download.headers.get("content-type"), mime);
        assert.deepEqual(Buffer.from(await download.arrayBuffer()), bytes);
        return result.previewHtml;
    } finally {
        URL.revokeObjectURL(result.downloadUrl);
    }
}

test("an empty worksheet renders an empty table instead of the preview error", async () => {
    const html = await preview("empty.xlsx");
    assert.doesNotMatch(html, /Excel preview failed:/);
    assert.match(html, /<table><\/table>/);
    assert.match(html, /<strong>Range:<\/strong> Empty/);
});

test("XLSX preview uses the first sheet and retains workbook metadata", async () => {
    const html = await preview("data.xlsx");
    assert.doesNotMatch(html, /Excel preview failed:/);
    assert.match(html, /<strong>Sheet:<\/strong> Data/);
    assert.match(html, /<strong>Range:<\/strong> A1:C3/);
    assert.match(html, /<strong>Cells:<\/strong> 9/);
    assert.match(html, /<strong>Available sheets:<\/strong> Data, Appendix/);
    assert.match(html, />Ada<\/td>/);
    assert.match(html, />12\.5<\/td>/);
    assert.doesNotMatch(html, /second-sheet-only/);
});

test("legacy XLS files still produce a preview and a downloadable workbook", async () => {
    const html = await preview("data.xls", "application/vnd.ms-excel");
    assert.doesNotMatch(html, /Excel preview failed:/);
    assert.match(html, /<strong>Range:<\/strong> A1:C3/);
    assert.match(html, />Ada<\/td>/);
    assert.match(html, />12\.5<\/td>/);
});

test("Unicode, multiline text, and quotes remain visible in the preview", async () => {
    const html = await preview("text.xlsx");
    assert.doesNotMatch(html, /Excel preview failed:/);
    assert.match(html, />São Paulo 東京 😀<\/td>/);
    assert.match(html, />line 1<br\/>line 2, &quot;quoted&quot;<\/td>/);
});
