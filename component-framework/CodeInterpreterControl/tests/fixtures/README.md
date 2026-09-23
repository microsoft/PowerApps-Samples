# Excel preview fixtures

These workbooks were generated with the published `xlsx@0.18.5` package so
preview tests read files produced before the dependency migration.

- `empty.xlsx`: one empty worksheet named `Data`.
- `data.xlsx` and `data.xls`: a `Data` worksheet containing
  `[["Name", "Amount", "Active"], ["Ada", 12.5, true], ["Grace", 0, false]]`,
  followed by an `Appendix` worksheet containing `[["second-sheet-only"]]`.
- `text.xlsx`: a `Data` worksheet containing
  `[["Name", "Notes"], ["São Paulo 東京 😀", "line 1\nline 2, \"quoted\""]]`.
  The `\n` in the description represents one newline in the cell.

The tests call the real `FilePreviewHandler.generatePreview` method and verify
both HTML output and the bytes served through the generated download URL.
