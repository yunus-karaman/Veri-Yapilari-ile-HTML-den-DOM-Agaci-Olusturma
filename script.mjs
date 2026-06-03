import {
  analyzeSubtree,
  buildDomTree,
  calculateDepth,
  flattenNodesWithDepth,
  formatNodeLabel,
  generateSyntheticHtml,
  getSiblings,
  resolveSearchPlan,
  searchTree,
} from "./dom-core.mjs";

const SAMPLE_HTML = `<html>
  <body>
    <header id="header" class="hero shell">
      <nav class="menu">
        <a id="brand" class="menu-item">Veri Yapıları</a>
        <a class="menu-item">Projeler</a>
      </nav>
    </header>
    <main class="container">
      <section class="card">
        <h1>DOM Ağacı</h1>
        <p class="lead">Bu panel hiyerarşiyi gösterir.</p>
      </section>
      <section class="card">
        <ul class="items">
          <li class="item">Queue</li>
          <li class="item">Stack</li>
          <li class="item">Hash Table</li>
        </ul>
      </section>
    </main>
  </body>
</html>`;

const elements = {
  htmlInput: document.querySelector("#html-input"),
  lineNumbers: document.querySelector("#line-numbers"),
  parseButton: document.querySelector("#parse-dom"),
  sampleButton: document.querySelector("#load-sample"),
  generateButton: document.querySelector("#generate-html"),
  syntheticCount: document.querySelector("#synthetic-count"),
  resetButton: document.querySelector("#reset-editor"),
  searchForm: document.querySelector(".search-controls"),
  searchInput: document.querySelector("#search-input"),
  strategy: document.querySelector("#search-strategy"),
  treeRoot: document.querySelector("#tree-root"),
  statusMessage: document.querySelector("#status-message"),
  searchSummary: document.querySelector("#search-summary"),
  nodeCount: document.querySelector("#node-count"),
  maxDepth: document.querySelector("#max-depth"),
  textCount: document.querySelector("#text-count"),
  details: document.querySelector("#node-details"),
};

const state = {
  root: null,
  idIndex: null,
  uidIndex: null,
  matches: [],
  selectedNodeId: null,
};

function updateLineNumbers() {
  const totalLines = elements.htmlInput.value.split("\n").length;
  elements.lineNumbers.textContent = Array.from({ length: totalLines }, (_, index) => `${index + 1}`).join("\n");
}

function setStatus(message, type = "neutral") {
  elements.statusMessage.textContent = message;
  elements.statusMessage.className = `status-pill ${type}`;
}

function updateMetrics(root) {
  if (!root) {
    elements.nodeCount.textContent = "0";
    elements.maxDepth.textContent = "0";
    elements.textCount.textContent = "0";
    return;
  }

  const nodes = flattenNodesWithDepth(root).filter(({ node }) => node.uid !== root.uid);
  const maxDepth = nodes.reduce((depth, item) => Math.max(depth, item.depth), 0);
  const textCount = nodes.filter(({ node }) => node.type === "text").length;

  elements.nodeCount.textContent = String(nodes.length);
  elements.maxDepth.textContent = String(maxDepth);
  elements.textCount.textContent = String(textCount);
}

function createNodeBadge(text) {
  const badge = document.createElement("span");
  badge.className = "node-badge";
  badge.textContent = text;
  return badge;
}

function nodeElementClass(node, matchIds, selectedNodeId) {
  const classes = [node.children.length > 0 ? "tree-branch" : "tree-leaf"];

  if (matchIds.has(node.uid)) {
    classes.push("is-match");
  }

  if (selectedNodeId === node.uid) {
    classes.push("is-selected");
  }

  return classes.join(" ");
}

function selectedPathIds(selectedNodeId) {
  const ids = new Set();
  let cursor = state.uidIndex?.get(selectedNodeId) || null;

  while (cursor) {
    ids.add(cursor.uid);
    cursor = cursor.parent;
  }

  return ids;
}

function createLabelContent(node) {
  const fragment = document.createDocumentFragment();

  if (node.type === "text") {
    const text = document.createElement("span");
    text.className = "node-text";
    text.textContent = formatNodeLabel(node);
    fragment.append(text);
    return fragment;
  }

  const tag = document.createElement("span");
  tag.className = "node-tag";
  tag.textContent = `<${node.tagName}>`;
  fragment.append(tag);

  if (node.id) {
    fragment.append(createNodeBadge(`#${node.id}`));
  }

  if (node.classList.length > 0) {
    fragment.append(createNodeBadge(`.${node.classList.join(".")}`));
  }

  return fragment;
}

function renderTreeNode(node, matchIds, selectedNodeId, selectedPath, depth = 0) {
  function createShell(current, currentDepth) {
    if (current.children.length === 0) {
      const leaf = document.createElement("div");
      leaf.className = nodeElementClass(current, matchIds, selectedNodeId);
      leaf.dataset.nodeId = current.uid;
      leaf.append(createLabelContent(current));
      return leaf;
    }

    const details = document.createElement("details");
    details.className = nodeElementClass(current, matchIds, selectedNodeId);
    details.dataset.nodeId = current.uid;
    details.open = currentDepth < 2 ||
      matchIds.has(current.uid) ||
      selectedPath.has(current.uid);

    const summary = document.createElement("summary");
    summary.append(createLabelContent(current));
    details.append(summary);
    return details;
  }

  const rootElement = createShell(node, depth);
  const stack = node.children.length > 0
    ? [{ node, element: rootElement, depth, childIndex: 0 }]
    : [];

  while (stack.length > 0) {
    const frame = stack[stack.length - 1];

    if (frame.childIndex >= frame.node.children.length) {
      stack.pop();
      continue;
    }

    const child = frame.node.children[frame.childIndex];
    frame.childIndex += 1;
    const childElement = createShell(child, frame.depth + 1);
    frame.element.append(childElement);

    if (child.children.length > 0) {
      stack.push({
        node: child,
        element: childElement,
        depth: frame.depth + 1,
        childIndex: 0,
      });
    }
  }

  return rootElement;
}

function renderTree() {
  elements.treeRoot.innerHTML = "";

  if (!state.root || state.root.children.length === 0) {
    const emptyState = document.createElement("div");
    emptyState.className = "empty-state";
    emptyState.innerHTML = "<div><strong>Ağaç henüz oluşturulmadı.</strong><p>Soldaki HTML metnini ayrıştırarak burada görüntüleyebilirsin.</p></div>";
    elements.treeRoot.append(emptyState);
    updateNodeDetails(null);
    return;
  }

  const matchIds = new Set(state.matches.map((match) => match.uid));
  const pathIds = selectedPathIds(state.selectedNodeId);

  for (const child of state.root.children) {
    elements.treeRoot.append(renderTreeNode(child, matchIds, state.selectedNodeId, pathIds, 1));
  }
}

function firstVisibleNode(root) {
  return root?.children?.[0] || null;
}

function updateNodeDetails(node) {
  const rows = elements.details.querySelectorAll("dd");

  if (!node) {
    rows.forEach((row) => {
      row.textContent = "-";
    });
    return;
  }

  const subtree = analyzeSubtree(node);

  rows[0].textContent = node.tagName;
  rows[1].textContent = node.id || "-";
  rows[2].textContent = node.classList.join(", ") || "-";
  rows[3].textContent = String(calculateDepth(node));
  rows[4].textContent = String(getSiblings(node).length);
  rows[5].textContent = String(subtree.size);
}

function selectNode(nodeId) {
  const node = state.uidIndex?.get(nodeId) || null;
  state.selectedNodeId = node ? nodeId : null;
  updateNodeDetails(node);
  renderTree();
}

function expandAncestors(node) {
  let cursor = node?.parent;

  while (cursor) {
    const element = elements.treeRoot.querySelector(`[data-node-id="${cursor.uid}"]`);
    if (element?.tagName === "DETAILS") {
      element.open = true;
    }
    cursor = cursor.parent;
  }
}

function runSearch() {
  if (!state.root) {
    setStatus("Önce DOM ağacını oluştur.", "error");
    return;
  }

  const query = elements.searchInput.value.trim();
  const plan = resolveSearchPlan(query, elements.strategy.value);
  if (!query) {
    state.matches = [];
    state.selectedNodeId = null;
    updateNodeDetails(null);
    elements.searchSummary.textContent = "Arama temizlendi";
    renderTree();
    return;
  }

  const matches = searchTree(state.root, state.idIndex, query, elements.strategy.value);
  state.matches = matches;
  const strategyText = plan ? `Strateji: ${plan.label} ${plan.complexity}` : "Strateji yok";

  if (matches.length === 0) {
    state.selectedNodeId = null;
    updateNodeDetails(null);
    elements.searchSummary.textContent = `Eşleşme bulunamadı: ${query}. ${strategyText}`;
    setStatus("Arama tamamlandı", "neutral");
    renderTree();
    return;
  }

  elements.searchSummary.textContent = `${matches.length} eşleşme bulundu. ${strategyText}`;
  setStatus("Arama tamamlandı", "neutral");
  state.selectedNodeId = matches[0].uid;
  updateNodeDetails(matches[0]);
  renderTree();
  expandAncestors(matches[0]);
  const firstElement = elements.treeRoot.querySelector(`[data-node-id="${matches[0].uid}"]`);
  firstElement?.scrollIntoView({ behavior: "smooth", block: "center" });
}

function parseAndRender() {
  try {
    const result = buildDomTree(elements.htmlInput.value);
    state.root = result.root;
    state.idIndex = result.idIndex;
    state.uidIndex = result.uidIndex;
    state.matches = [];
    const selectedNode = firstVisibleNode(result.root);
    state.selectedNodeId = selectedNode?.uid || null;
    updateMetrics(result.root);
    updateNodeDetails(selectedNode);
    renderTree();
    elements.searchSummary.textContent = `${result.tokens.length} token ayrıldı`;
    setStatus("DOM ağacı oluşturuldu", "neutral");
  } catch (error) {
    state.root = null;
    state.idIndex = null;
    state.uidIndex = null;
    state.matches = [];
    state.selectedNodeId = null;
    updateMetrics(null);
    renderTree();
    elements.searchSummary.textContent = "Sonuç yok";
    setStatus(error.message, "error");
  }
}

elements.htmlInput.addEventListener("input", updateLineNumbers);
elements.htmlInput.addEventListener("scroll", () => {
  elements.lineNumbers.scrollTop = elements.htmlInput.scrollTop;
});

elements.parseButton.addEventListener("click", parseAndRender);
elements.sampleButton.addEventListener("click", () => {
  elements.htmlInput.value = SAMPLE_HTML;
  updateLineNumbers();
  parseAndRender();
});

elements.generateButton.addEventListener("click", () => {
  const count = Math.max(1, Math.min(Number.parseInt(elements.syntheticCount.value, 10) || 1, 1000));
  elements.syntheticCount.value = String(count);
  elements.htmlInput.value = generateSyntheticHtml(count);
  updateLineNumbers();
  parseAndRender();
  elements.searchInput.value = count > 1 ? ".synthetic-node" : "#node-1";
});

elements.resetButton.addEventListener("click", () => {
  elements.htmlInput.value = "";
  elements.searchInput.value = "";
  updateLineNumbers();
  state.root = null;
  state.idIndex = null;
  state.uidIndex = null;
  state.matches = [];
  state.selectedNodeId = null;
  updateMetrics(null);
  renderTree();
  setStatus("Editor temizlendi", "neutral");
  elements.searchSummary.textContent = "Sonuç yok";
});

elements.searchForm.addEventListener("submit", (event) => {
  event.preventDefault();
  runSearch();
});

elements.treeRoot.addEventListener("click", (event) => {
  const target = event.target.closest("[data-node-id]");
  if (!target || !state.root) {
    return;
  }

  selectNode(target.dataset.nodeId);
});

function initialize() {
  try {
    elements.htmlInput.value = SAMPLE_HTML;
    updateLineNumbers();
    parseAndRender();
  } catch (error) {
    setStatus(`Başlangıç örneği yüklenemedi: ${error.message}`, "error");
  }
}

initialize();
