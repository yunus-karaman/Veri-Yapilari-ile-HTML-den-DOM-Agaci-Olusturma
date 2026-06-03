const VOID_ELEMENTS = new Set([
  "area",
  "base",
  "br",
  "col",
  "embed",
  "hr",
  "img",
  "input",
  "link",
  "meta",
  "param",
  "source",
  "track",
  "wbr",
]);

const RAW_TEXT_ELEMENTS = new Set([
  "script",
  "style",
  "textarea",
  "title",
]);

class StackNode {
  constructor(value, next = null) {
    this.value = value;
    this.next = next;
  }
}

class QueueNode {
  constructor(value) {
    this.value = value;
    this.next = null;
  }
}

class HashEntry {
  constructor(key, value, next = null) {
    this.key = key;
    this.value = value;
    this.next = next;
  }
}

export class Stack {
  constructor() {
    this.topNode = null;
    this.count = 0;
  }

  push(value) {
    this.topNode = new StackNode(value, this.topNode);
    this.count += 1;
  }

  pop() {
    if (!this.topNode) {
      throw new Error("Stack boş.");
    }

    const { value } = this.topNode;
    this.topNode = this.topNode.next;
    this.count -= 1;
    return value;
  }

  peek() {
    return this.topNode ? this.topNode.value : null;
  }

  isEmpty() {
    return this.count === 0;
  }
}

export class Queue {
  constructor() {
    this.head = null;
    this.tail = null;
    this.count = 0;
  }

  enqueue(value) {
    const node = new QueueNode(value);
    if (!this.tail) {
      this.head = node;
      this.tail = node;
    } else {
      this.tail.next = node;
      this.tail = node;
    }
    this.count += 1;
  }

  dequeue() {
    if (!this.head) {
      throw new Error("Queue boş.");
    }

    const { value } = this.head;
    this.head = this.head.next;
    if (!this.head) {
      this.tail = null;
    }
    this.count -= 1;
    return value;
  }

  isEmpty() {
    return this.count === 0;
  }
}

export class HashTable {
  constructor(capacity = 127) {
    this.capacity = capacity;
    this.buckets = new Array(capacity).fill(null);
  }

  hash(key) {
    let total = 0;
    for (let index = 0; index < key.length; index += 1) {
      total = (total * 31 + key.charCodeAt(index)) % this.capacity;
    }
    return total;
  }

  set(key, value) {
    const index = this.hash(key);
    let current = this.buckets[index];

    while (current) {
      if (current.key === key) {
        current.value = value;
        return;
      }
      current = current.next;
    }

    this.buckets[index] = new HashEntry(key, value, this.buckets[index]);
  }

  get(key) {
    let current = this.buckets[this.hash(key)];

    while (current) {
      if (current.key === key) {
        return current.value;
      }
      current = current.next;
    }

    return null;
  }
}

function createElementNode(createUid, tagName, attributes = {}) {
  const classList = attributes.class
    ? attributes.class.split(/\s+/).filter(Boolean)
    : [];

  return {
    uid: createUid(),
    type: "element",
    tagName: tagName.toLowerCase(),
    attributes,
    id: attributes.id || "",
    classList,
    textContent: "",
    children: [],
    parent: null,
  };
}

function createTextNode(createUid, textContent) {
  return {
    uid: createUid(),
    type: "text",
    tagName: "#text",
    attributes: {},
    id: "",
    classList: [],
    textContent,
    children: [],
    parent: null,
  };
}

function addChild(parent, child) {
  child.parent = parent;
  parent.children.push(child);
}

function parseAttributes(rawTag) {
  const attributes = {};
  const source = rawTag.replace(/^[^\s/]+/, "");
  const attributePattern = /([A-Za-z_:][-A-Za-z0-9_:.]*)(?:\s*=\s*("([^"]*)"|'([^']*)'|([^\s"'=<>`]+)))?/g;
  let match;

  while ((match = attributePattern.exec(source)) !== null) {
    attributes[match[1]] = match[3] ?? match[4] ?? match[5] ?? "";
  }

  return attributes;
}

function isTagNameStart(character) {
  return /[A-Za-z]/.test(character);
}

function readTagAt(html, startIndex) {
  if (html[startIndex] !== "<") {
    return null;
  }

  if (html.startsWith("<!--", startIndex)) {
    const endIndex = html.indexOf("-->", startIndex + 4);
    return endIndex === -1
      ? { value: html.slice(startIndex), endIndex: html.length }
      : { value: html.slice(startIndex, endIndex + 3), endIndex: endIndex + 3 };
  }

  if (html[startIndex + 1] === "!") {
    const endIndex = html.indexOf(">", startIndex + 2);
    return endIndex === -1
      ? { value: html.slice(startIndex), endIndex: html.length }
      : { value: html.slice(startIndex, endIndex + 1), endIndex: endIndex + 1 };
  }

  const nameIndex = html[startIndex + 1] === "/" ? startIndex + 2 : startIndex + 1;
  if (!isTagNameStart(html[nameIndex] || "")) {
    return null;
  }

  let quote = null;
  for (let index = nameIndex + 1; index < html.length; index += 1) {
    const character = html[index];

    if (quote) {
      if (character === quote) {
        quote = null;
      }
      continue;
    }

    if (character === "\"" || character === "'") {
      quote = character;
      continue;
    }

    if (character === ">") {
      return { value: html.slice(startIndex, index + 1), endIndex: index + 1 };
    }
  }

  return null;
}

function addTextToken(tokens, value) {
  if (value.trim()) {
    tokens.push({ type: "text", value });
  }
}

function findRawTextClose(html, startIndex, tagName) {
  const closePattern = new RegExp(`</\\s*${tagName}\\s*>`, "i");
  const match = closePattern.exec(html.slice(startIndex));

  if (!match) {
    return null;
  }

  return {
    startIndex: startIndex + match.index,
    endIndex: startIndex + match.index + match[0].length,
    value: match[0],
  };
}

export function tokenize(html) {
  const tokens = [];
  let index = 0;
  let textStart = 0;

  while (index < html.length) {
    if (html[index] !== "<") {
      index += 1;
      continue;
    }

    const tag = readTagAt(html, index);
    if (!tag) {
      index += 1;
      continue;
    }

    addTextToken(tokens, html.slice(textStart, index));
    const { value } = tag;

    if (value.startsWith("<!--")) {
      index = tag.endIndex;
      textStart = index;
      continue;
    }

    if (value.startsWith("<!")) {
      index = tag.endIndex;
      textStart = index;
      continue;
    }

    if (value.startsWith("</")) {
      tokens.push({ type: "closeTag", value });
      index = tag.endIndex;
      textStart = index;
      continue;
    }

    const { tagName } = parseTag(value);
    const tokenType = value.endsWith("/>") || VOID_ELEMENTS.has(tagName) ? "selfClosingTag" : "openTag";
    tokens.push({ type: tokenType, value });
    index = tag.endIndex;
    textStart = index;

    if (tokenType === "openTag" && RAW_TEXT_ELEMENTS.has(tagName)) {
      const closeTag = findRawTextClose(html, index, tagName);
      if (!closeTag) {
        addTextToken(tokens, html.slice(index));
        index = html.length;
        textStart = index;
        break;
      }

      addTextToken(tokens, html.slice(index, closeTag.startIndex));
      tokens.push({ type: "closeTag", value: closeTag.value });
      index = closeTag.endIndex;
      textStart = index;
    }
  }

  addTextToken(tokens, html.slice(textStart));
  return tokens;
}

function parseTag(token) {
  const normalized = token
    .replace(/^</, "")
    .replace(/\/?>$/, "")
    .trim();

  if (!normalized) {
    throw new Error("Geçersiz HTML etiketi.");
  }

  const parts = normalized.split(/\s+/);
  const tagName = parts[0].toLowerCase();

  return {
    tagName,
    attributes: parseAttributes(normalized),
  };
}

export function buildDomTree(html) {
  let nextNodeId = 0;
  const createUid = () => `node-${nextNodeId += 1}`;
  const tokens = tokenize(html);
  const root = createElementNode(createUid, "document");
  const stack = new Stack();
  const idIndex = new HashTable(211);
  const uidIndex = new HashTable(503);

  uidIndex.set(root.uid, root);

  stack.push(root);

  for (const token of tokens) {
    if (token.type === "text") {
      const textNode = createTextNode(createUid, token.value.trim());
      addChild(stack.peek(), textNode);
      uidIndex.set(textNode.uid, textNode);
      continue;
    }

    if (token.type === "openTag" || token.type === "selfClosingTag") {
      const { tagName, attributes } = parseTag(token.value);
      const node = createElementNode(createUid, tagName, attributes);
      addChild(stack.peek(), node);
      uidIndex.set(node.uid, node);

      if (node.id) {
        idIndex.set(node.id, node);
      }

      if (token.type === "openTag") {
        stack.push(node);
      }
      continue;
    }

    const closingTag = token.value.replace(/^<\//, "").replace(/>$/, "").trim().toLowerCase();
    if (stack.count === 1) {
      throw new Error(`Etiket uyuşmazlığı: </${closingTag}> beklenmeyen konumda.`);
    }
    const current = stack.pop();

    if (!current || current.tagName.toLowerCase() !== closingTag.toLowerCase()) {
      throw new Error(`Etiket uyuşmazlığı: </${closingTag}> beklenmeyen konumda.`);
    }
  }

  if (stack.count !== 1) {
    throw new Error("HTML yapısı tamamlanmamış. Açık kalan etiketler var.");
  }

  return {
    root,
    idIndex,
    uidIndex,
    tokens,
  };
}

export function breadthFirstSearch(root, predicate) {
  const matches = [];
  const queue = new Queue();
  queue.enqueue(root);

  while (!queue.isEmpty()) {
    const node = queue.dequeue();

    if (predicate(node)) {
      matches.push(node);
    }

    for (const child of node.children) {
      queue.enqueue(child);
    }
  }

  return matches;
}

export function depthFirstSearch(root, predicate, matches = []) {
  if (!root) {
    return matches;
  }

  const stack = [root];
  while (stack.length > 0) {
    const node = stack.pop();

    if (predicate(node)) {
      matches.push(node);
    }

    for (let index = node.children.length - 1; index >= 0; index -= 1) {
      stack.push(node.children[index]);
    }
  }

  return matches;
}

export function calculateDepth(node) {
  let depth = 0;
  let cursor = node;

  while (cursor?.parent) {
    depth += 1;
    cursor = cursor.parent;
  }

  return depth;
}

export function getSiblings(node) {
  if (!node?.parent) {
    return [];
  }

  return node.parent.children.filter((child) => child.uid !== node.uid);
}

export function analyzeSubtree(node) {
  if (!node) {
    return { size: 0, textNodes: 0, deepest: 0 };
  }

  let size = 0;
  let textNodes = 0;
  let deepest = calculateDepth(node);
  const stack = [node];

  while (stack.length > 0) {
    const current = stack.pop();
    size += 1;

    if (current.type === "text") {
      textNodes += 1;
    }

    deepest = Math.max(deepest, calculateDepth(current));

    for (const child of current.children) {
      stack.push(child);
    }
  }

  return { size, textNodes, deepest };
}

export function flattenNodes(root) {
  return depthFirstSearch(root, () => true, []);
}

export function flattenNodesWithDepth(root) {
  const nodes = [];
  const stack = root ? [{ node: root, depth: 0 }] : [];

  while (stack.length > 0) {
    const { node, depth } = stack.pop();
    nodes.push({ node, depth });

    for (let index = node.children.length - 1; index >= 0; index -= 1) {
      stack.push({ node: node.children[index], depth: depth + 1 });
    }
  }

  return nodes;
}

export function parseSearchQuery(query) {
  const normalized = query.trim();

  if (!normalized) {
    return null;
  }

  let match = normalized.match(/^id\s*=\s*"([^"]+)"$/i);
  if (match) {
    return { mode: "id", value: match[1] };
  }

  match = normalized.match(/^class\s*=\s*"([^"]+)"$/i);
  if (match) {
    return { mode: "class", value: match[1] };
  }

  match = normalized.match(/^tag\s*=\s*"?(.*?)"?$/i);
  if (match && match[1]) {
    return { mode: "tag", value: match[1] };
  }

  if (normalized.startsWith("#")) {
    return { mode: "id", value: normalized.slice(1) };
  }

  if (normalized.startsWith(".")) {
    return { mode: "class", value: normalized.slice(1) };
  }

  return { mode: "tag", value: normalized };
}

export function resolveSearchPlan(query, strategy = "auto") {
  const parsed = parseSearchQuery(query);

  if (!parsed) {
    return null;
  }

  if (parsed.mode === "id" && strategy === "auto") {
    return {
      parsed,
      label: "Hash indeks",
      complexity: "O(1)",
      traversal: null,
      usesIndex: true,
    };
  }

  const traversal = strategy === "dfs" ? "DFS" : "BFS";

  return {
    parsed,
    label: traversal,
    complexity: "O(n)",
    traversal: traversal.toLowerCase(),
    usesIndex: false,
  };
}

export function searchTree(root, idIndex, query, strategy = "auto") {
  const plan = resolveSearchPlan(query, strategy);

  if (!plan) {
    return [];
  }

  const { parsed } = plan;

  if (plan.usesIndex) {
    const match = idIndex.get(parsed.value);
    return match ? [match] : [];
  }

  if (parsed.mode === "id") {
    console.warn(
      `ID araması ${plan.label} ile ${plan.complexity} tarama yapıyor. O(1) hash indeks için Otomatik stratejiyi seçin.`,
    );
  }

  const predicate = (node) => {
    if (parsed.mode === "id") {
      return node.id === parsed.value;
    }

    if (parsed.mode === "class") {
      return node.classList.includes(parsed.value);
    }

    return node.tagName.toLowerCase() === parsed.value.toLowerCase();
  };

  if (plan.traversal === "dfs") {
    return depthFirstSearch(root, predicate);
  }

  return breadthFirstSearch(root, predicate);
}

export function formatNodeLabel(node) {
  if (node.type === "text") {
    const text = node.textContent.length > 42
      ? `${node.textContent.slice(0, 39)}...`
      : node.textContent;
    return `"${text}"`;
  }

  return node.tagName;
}

export function generateSyntheticHtml(nodeCount) {
  const count = Math.max(1, Math.min(Number.parseInt(nodeCount, 10) || 1, 1000));
  const nodes = Array.from({ length: count }, (_, index) => ({
    index,
    id: `node-${index + 1}`,
    children: [],
  }));

  for (let index = 1; index < nodes.length; index += 1) {
    const parentIndex = Math.floor((index - 1) / 3);
    nodes[parentIndex].children.push(nodes[index]);
  }

  function renderNode(node) {
    const tagName = node.index === 0 ? "main" : "section";
    const className = node.index === 0 ? "synthetic-root" : "synthetic-node";
    const children = node.children
      .map((child) => renderNode(child))
      .join("");

    return `<${tagName} id="${node.id}" class="${className}">${children}</${tagName}>`;
  }

  return renderNode(nodes[0]);
}
