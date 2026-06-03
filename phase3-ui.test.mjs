import assert from "node:assert/strict";

import {
  HashTable,
  Queue,
  Stack,
  analyzeSubtree,
  breadthFirstSearch,
  buildDomTree,
  depthFirstSearch,
  flattenNodesWithDepth,
  generateSyntheticHtml,
  parseSearchQuery,
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

const multiLine = buildDomTree(`<div
  id="main"
  class="container box">
  Merhaba
</div>`);
const multiLineDiv = multiLine.root.children[0];
assert.equal(multiLineDiv.id, "main");
assert.deepEqual(multiLineDiv.classList, ["container", "box"]);

const tabbed = buildDomTree("<div\tid=\"tabbed\"\tclass=\"a b\"></div>");
const tabbedDiv = tabbed.root.children[0];
assert.equal(tabbedDiv.id, "tabbed");
assert.deepEqual(tabbedDiv.classList, ["a", "b"]);

const voidElements = buildDomTree("<div><br><img src=\"x.png\"><input></div>");
assert.deepEqual(voidElements.root.children[0].children.map((node) => node.tagName), ["br", "img", "input"]);

const mixedCase = buildDomTree("<DIV id=\"hero\"><SPAN>Text</span></div>");
const mixedDiv = mixedCase.root.children[0];
assert.equal(mixedDiv.tagName, "div");
assert.equal(mixedDiv.children[0].tagName, "span");
assert.equal(mixedDiv.children[0].children[0].type, "text");

assert.throws(() => buildDomTree("<div><p></div>"), /Etiket uyuşmazlığı/);
assert.throws(() => buildDomTree("<div><p>test</p>"), /tamamlanmamış/);

const first = buildDomTree("<section></section>");
const second = buildDomTree("<article></article>");
assert.equal(first.root.uid, "node-1");
assert.equal(second.root.uid, "node-1");

const depths = flattenNodesWithDepth(mixedCase.root).map(({ node, depth }) => [node.tagName, depth]);
assert.deepEqual(depths, [
  ["document", 0],
  ["div", 1],
  ["span", 2],
  ["#text", 3],
]);

assert.deepEqual(resolveSearchPlan("#hero", "auto"), {
  parsed: { mode: "id", value: "hero" },
  label: "Hash indeks",
  complexity: "O(1)",
  traversal: null,
  usesIndex: true,
});

assert.deepEqual(parseSearchQuery("id=\"header\""), { mode: "id", value: "header" });
assert.deepEqual(parseSearchQuery("class=\"container\""), { mode: "class", value: "container" });
assert.deepEqual(parseSearchQuery("tag=\"section\""), { mode: "tag", value: "section" });
assert.deepEqual(parseSearchQuery(".container"), { mode: "class", value: "container" });
assert.equal(resolveSearchPlan("#hero", "bfs").label, "BFS");

const searchFixture = buildDomTree(`
  <main id="app">
    <section class="card primary"></section>
    <SECTION class="card"></SECTION>
    <article class="Card"></article>
  </main>
`);
assert.equal(searchTree(searchFixture.root, searchFixture.idIndex, "#app", "auto").length, 1);
assert.equal(searchTree(searchFixture.root, searchFixture.idIndex, ".card", "bfs").length, 3);
assert.equal(searchTree(searchFixture.root, searchFixture.idIndex, ".card", "dfs").length, 3);
assert.equal(searchTree(searchFixture.root, searchFixture.idIndex, "section", "dfs").length, 2);

const rehashTable = new HashTable(2);
for (let index = 0; index < 25; index += 1) {
  rehashTable.set(`key-${index}`, { index });
}
assert.equal(rehashTable.capacity > 2, true);
assert.equal(rehashTable.count, 25);
for (let index = 0; index < 25; index += 1) {
  assert.equal(rehashTable.get(`key-${index}`).index, index);
}
rehashTable.set("key-7", { index: 700 });
assert.equal(rehashTable.count, 25);
assert.equal(rehashTable.get("key-7").index, 700);
assert.equal(rehashTable.remove("key-7"), true);
assert.equal(rehashTable.get("key-7"), null);
assert.equal(rehashTable.remove("key-7"), false);
assert.throws(() => rehashTable.set("", {}), /anahtarı/);

const stack = new Stack();
stack.push("first");
stack.push("second");
assert.equal(stack.pop(), "second");
assert.equal(stack.pop(), "first");
assert.equal(stack.isEmpty(), true);

const queue = new Queue();
queue.enqueue("first");
queue.enqueue("second");
assert.equal(queue.dequeue(), "first");
assert.equal(queue.dequeue(), "second");
assert.equal(queue.isEmpty(), true);

const traversalFixture = buildDomTree("<main><section id=\"a\"><p id=\"b\"></p></section><aside id=\"c\"></aside></main>");
const dfsOrder = depthFirstSearch(traversalFixture.root, (node) => node.type === "element")
  .map((node) => node.tagName);
const bfsOrder = breadthFirstSearch(traversalFixture.root, (node) => node.type === "element")
  .map((node) => node.tagName);
assert.deepEqual(dfsOrder, ["document", "main", "section", "p", "aside"]);
assert.deepEqual(bfsOrder, ["document", "main", "section", "aside", "p"]);

const subtree = analyzeSubtree(traversalFixture.root.children[0]);
assert.equal(subtree.size, 4);
assert.equal(subtree.textNodes, 0);
assert.equal(subtree.height, 2);
assert.equal(subtree.depth, 1);

for (const count of [1, 10, 100, 500, 1000]) {
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
