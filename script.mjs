import {
  analyzeSubtree,
  buildDomTree,
  calculateDepth,
  flattenNodesWithDepth,
  formatNodeLabel,
  getSiblings,
  resolveSearchPlan,
  searchTree,
} from "./dom-core.mjs";

const SAMPLE_HTML = `<html>
  <body>
    <header id="header" class="hero shell">
      <nav class="menu">
        <a id="brand" class="menu-item">Veri Yapilari</a>
        <a class="menu-item">Projeler</a>
      </nav>
    </header>
    <main class="container">
      <section class="card">
        <h1>DOM Agaci</h1>
        <p class="lead">Bu panel hiyerarsiyi gosterir.</p>
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

function nodeElementClass(node, matches, selectedNodeId) {
  const classes = [node.children.length > 0 ? "tree-branch" : "tree-leaf"];

  if (matches.some((match) => match.uid === node.uid)) {
    classes.push("is-match");
  }

  if (selectedNodeId === node.uid) {
    classes.push("is-selected");
  }

  return classes.join(" ");
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

function renderTreeNode(node, matches, selectedNodeId, depth = 0) {
  const hasChildren = node.children.length > 0;

  if (hasChildren) {
    const details = document.createElement("details");
    details.className = nodeElementClass(node, matches, selectedNodeId);
    details.dataset.nodeId = node.uid;
    details.open = depth < 2 || matches.some((match) => match.uid === node.uid);

    const summary = document.createElement("summary");
    summary.dataset.nodeId = node.uid;
    summary.append(createLabelContent(node));
    details.append(summary);

    for (const child of node.children) {
      details.append(renderTreeNode(child, matches, selectedNodeId, depth + 1));
    }

    return details;
  }

  const leaf = document.createElement("div");
  leaf.className = nodeElementClass(node, matches, selectedNodeId);
  leaf.dataset.nodeId = node.uid;
  leaf.append(createLabelContent(node));
  return leaf;
}

function renderTree() {
  elements.treeRoot.innerHTML = "";

  if (!state.root || state.root.children.length === 0) {
    const emptyState = document.createElement("div");
    emptyState.className = "empty-state";
    emptyState.innerHTML = "<div><strong>Agac henuz olusturulmadi.</strong><p>Soldaki HTML metnini ayrisirarak burada gosterebilirsin.</p></div>";
    elements.treeRoot.append(emptyState);
    updateNodeDetails(null);
    return;
  }

  for (const child of state.root.children) {
    elements.treeRoot.append(renderTreeNode(child, state.matches, state.selectedNodeId, 1));
  }
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
    setStatus("Once DOM agacini olustur.", "error");
    return;
  }

  const query = elements.searchInput.value.trim();
  const plan = resolveSearchPlan(query, elements.strategy.value);
  if (!query) {
    state.matches = [];
    elements.searchSummary.textContent = "Arama temizlendi";
    renderTree();
    return;
  }

  const matches = searchTree(state.root, state.idIndex, query, elements.strategy.value);
  state.matches = matches;
  const strategyText = plan ? `Strateji: ${plan.label} ${plan.complexity}` : "Strateji yok";

  if (matches.length === 0) {
    elements.searchSummary.textContent = `Eslesme bulunamadi: ${query}. ${strategyText}`;
    setStatus("Arama tamamlandi", "neutral");
    renderTree();
    return;
  }

  elements.searchSummary.textContent = `${matches.length} eslesme bulundu. ${strategyText}`;
  setStatus("Arama tamamlandi", "neutral");
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
    state.selectedNodeId = null;
    updateMetrics(result.root);
    renderTree();
    elements.searchSummary.textContent = `${result.tokens.length} token ayrildi`;
    setStatus("DOM agaci olusturuldu", "neutral");
  } catch (error) {
    state.root = null;
    state.idIndex = null;
    state.uidIndex = null;
    state.matches = [];
    state.selectedNodeId = null;
    updateMetrics(null);
    renderTree();
    elements.searchSummary.textContent = "Sonuc yok";
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
  elements.searchSummary.textContent = "Sonuc yok";
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
    setStatus(`Baslangic ornegi yuklenemedi: ${error.message}`, "error");
  }
}

initialize();
