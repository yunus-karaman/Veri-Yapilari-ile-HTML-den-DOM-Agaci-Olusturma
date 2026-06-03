import assert from "node:assert/strict";

import {
  buildDomTree,
  flattenNodesWithDepth,
  generateSyntheticHtml,
  resolveSearchPlan,
  searchTree,
} from "./dom-core.mjs";

const sampleHtml = `<html>
  <body>
    <header id="header" class="hero shell">
      <nav class="menu">
        <a id="brand" class="menu-item">Veri Yapıları</a>
        <a class="menu-item">Projeler</a>
      </nav>
    </header>
  </body>
</html>`;

const sample = buildDomTree(sampleHtml);
assert.equal(sample.tokens.length > 0, true);
assert.equal(searchTree(sample.root, sample.idIndex, "#header", "auto").length, 1);
assert.equal(sample.uidIndex.get("node-1"), sample.root);

const mixedCase = buildDomTree("<DIV id=\"hero\"><BR><IMG src=\"hero.png\">Merhaba</div>");
const div = mixedCase.root.children[0];
assert.equal(div.tagName, "div");
assert.equal(div.children[0].tagName, "br");
assert.equal(div.children[1].tagName, "img");
assert.equal(div.children[2].type, "text");

const first = buildDomTree("<section></section>");
const second = buildDomTree("<article></article>");
assert.equal(first.root.uid, "node-1");
assert.equal(second.root.uid, "node-1");

const depths = flattenNodesWithDepth(mixedCase.root).map(({ node, depth }) => [node.tagName, depth]);
assert.deepEqual(depths, [
  ["document", 0],
  ["div", 1],
  ["br", 2],
  ["img", 2],
  ["#text", 2],
]);

assert.deepEqual(resolveSearchPlan("#hero", "auto"), {
  parsed: { mode: "id", value: "hero" },
  label: "Hash indeks",
  complexity: "O(1)",
  traversal: null,
  usesIndex: true,
});

assert.equal(resolveSearchPlan("#hero", "bfs").label, "BFS");

for (const count of [1, 10, 100, 500]) {
  const generated = buildDomTree(generateSyntheticHtml(count));
  const nodesWithoutDocument = flattenNodesWithDepth(generated.root)
    .filter(({ node }) => node.uid !== generated.root.uid);

  assert.equal(nodesWithoutDocument.length, count);
  assert.equal(searchTree(generated.root, generated.idIndex, `#node-${count}`, "auto").length, 1);

  if (count > 1) {
    assert.equal(searchTree(generated.root, generated.idIndex, ".synthetic-node", "bfs").length, count - 1);
  }
}

console.log("phase3 ui tests passed");
