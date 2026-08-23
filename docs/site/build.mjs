// Renders the docs site: DocFX ManagedReference yml (api/) + markdown pages (content/<lang>/) → _site/.
import fs from "node:fs";
import path from "node:path";
import YAML from "yaml";
import { marked } from "marked";

const ROOT = path.dirname(new URL(import.meta.url).pathname.replace(/^\/([A-Za-z]:)/, "$1"));
const API_DIR = path.join(ROOT, "api");
const CONTENT = path.join(ROOT, "content");
const OUT = path.join(ROOT, "_site");
const VERSION = "2.0.0-preview.4";
const PACKAGE_ORDER = ["McProtoNet", "McProtoNet.Transport", "McProtoNet.Protocol", "McProtoNet.Primitives", "McProtoNet.NBT"];

const LANGS = {
  en: {
    banner: "the library is in active development: some features are unstable, not every protocol packet is supported.",
    nav: { overview: "Project overview", nuget: "NuGet", github: "GitHub" },
    search: "Search the docs",
    groups: { intro: "Introduction", api: "API reference" },
    intro: [["index", "About the project"], ["getting-started", "First bot in 15 minutes"], ["non-goals", "What it does not do"], ["glossary", "Glossary"]],
    crumbs: { docs: "Documentation", api: "API reference" },
    sections: { ctor: "Constructors", prop: "Properties", method: "Methods", field: "Fields", event: "Events", ext: "Extension methods", types: "Types" },
    labels: { source: "Source", params: "Parameters", returns: "Returns", exceptions: "Exceptions", remarks: "Remarks", namespace: "Namespace", assembly: "Assembly", package: "Package", inherits: "Inheritance", implements: "Implements", derived: "Derived", noSummary: "—", onThisPage: "In this article", overloads: "Overloads", definition: "Definition", examples: "Examples", root: "(root namespace)", declaringType: "Declaring type" },
    apiIndexTitle: "API reference",
    apiIndexLead: "Generated from the XML documentation comments. Grouped by NuGet package, then namespace. Descriptions are English only for now.",
  },
  ru: {
    banner: "библиотека в активной разработке: часть функций нестабильна, поддержаны не все пакеты протокола.",
    nav: { overview: "Обзор проекта", nuget: "NuGet", github: "GitHub" },
    search: "Поиск по документации",
    groups: { intro: "Введение", api: "Справка API" },
    intro: [["index", "О проекте"], ["getting-started", "Первый бот за 15 минут"], ["non-goals", "Что не делает"], ["glossary", "Словарь"]],
    crumbs: { docs: "Документация", api: "Справка API" },
    sections: { ctor: "Конструкторы", prop: "Свойства", method: "Методы", field: "Поля", event: "События", ext: "Методы расширения", types: "Типы" },
    labels: { source: "Исходный код", params: "Параметры", returns: "Возвращает", exceptions: "Исключения", remarks: "Примечания", namespace: "Пространство имён", assembly: "Сборка", package: "Пакет", inherits: "Наследование", implements: "Реализует", derived: "Наследники", noSummary: "—", onThisPage: "На этой странице", overloads: "Перегрузки", definition: "Определение", examples: "Примеры", root: "(корневое пространство)", declaringType: "Объявлен в" },
    apiIndexTitle: "Справка API",
    apiIndexLead: "Собрана из XML-комментариев. Сгруппирована по NuGet-пакету, затем по пространству имён. Описания пока только на английском.",
  },
};
const TYPE_KINDS = ["Class", "Struct", "Interface", "Enum", "Delegate"];

// ---------- load API ----------
const items = new Map();
for (const f of fs.readdirSync(API_DIR)) {
  if (!f.endsWith(".yml") || f === "toc.yml") continue;
  const doc = YAML.parse(fs.readFileSync(path.join(API_DIR, f), "utf8"));
  if (!doc?.items) continue;
  for (const it of doc.items) items.set(it.uid, it);
}
const types = [...items.values()].filter(it => TYPE_KINDS.includes(it.type)).sort((a, b) => a.name.localeCompare(b.name));
// package → namespace → types
const tree = new Map();
for (const t of types) {
  const pkg = t.assemblies?.[0] ?? "?";
  const ns = t.namespace ?? "";
  if (!tree.has(pkg)) tree.set(pkg, new Map());
  if (!tree.get(pkg).has(ns)) tree.get(pkg).set(ns, []);
  tree.get(pkg).get(ns).push(t);
}
const packages = [...tree.keys()].sort((a, b) => (PACKAGE_ORDER.indexOf(a) + 1 || 99) - (PACKAGE_ORDER.indexOf(b) + 1 || 99) || a.localeCompare(b));
const pkgOf = t => t.assemblies?.[0] ?? "?";
// short name → uid, only when the short name is unique across the whole API (AbilitiesPacket exists in both directions)
const typeIndex = new Map();
{
  const count = new Map();
  for (const t of types) { const n = t.name.replace(/<.*$/, ""); count.set(n, (count.get(n) ?? 0) + 1); }
  for (const t of types) { const n = t.name.replace(/<.*$/, ""); if (count.get(n) === 1) typeIndex.set(n, t.uid); }
}

// ---------- helpers ----------
const esc = s => String(s ?? "").replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/"/g, "&quot;");
const slug = uid => uid.replace(/[^A-Za-z0-9_.]/g, "_").replace(/_+$/, "");
const typeHref = (lang, uid) => `/${lang}/api/${slug(uid)}.html`;
const overloadKey = m => (m.overload ?? m.uid).replace(/\*$/, "");
const memberHref = (lang, m) => `/${lang}/api/${slug(overloadKey(m))}.html`;
const other = lang => (lang === "en" ? "ru" : "en");
const nsLabel = (L, pkg, ns) => (ns.startsWith(pkg + ".") ? ns.slice(pkg.length + 1) : ns);

function kindLabel(it) {
  const c = it.syntax?.content ?? "";
  const m = c.match(/^(?:public\s+)?((?:static\s+|abstract\s+|sealed\s+|readonly\s+|ref\s+|partial\s+)*)(record struct|record|class|struct|interface|enum|delegate)\b/);
  return m ? (m[1] + m[2]).replace(/\s+/g, " ").trim() : it.type.toLowerCase();
}
function shortSig(it) {
  let c = (it.syntax?.content ?? it.name).replace(/\s+/g, " ").trim();
  c = c.replace(/^(public|private|protected|internal|protected internal|private protected)\s+/, "");
  c = c.replace(/\b(readonly|override|virtual|abstract|sealed|new|extern|unsafe|required)\s+/g, "");
  c = c.replace(/\s*\{\s*(get|set|init|add|remove)[^}]*\}\s*$/, "");
  c = c.replace(/\s*=\s*default\s*$/, "");
  return c;
}
function linkifySig(lang, sig) {
  return esc(sig).replace(/\b([A-Z][A-Za-z0-9_]*)\b/g, (m, name) => {
    const uid = typeIndex.get(name);
    return uid ? `<a href="${typeHref(lang, uid)}">${name}</a>` : m;
  });
}
function xrefToHtml(lang, text) {
  if (!text) return "";
  return String(text)
    .replace(/<xref href="([^"]+)"[^>]*><\/xref>/g, (_, uid) => {
      try { uid = decodeURIComponent(uid); } catch { /* keep */ }
      const it = items.get(uid);
      const label = it ? it.name : uid.replace(/\(.*$/, "").split(".").pop();
      if (it && TYPE_KINDS.includes(it.type)) return `<a href="${typeHref(lang, it.uid)}"><code>${esc(label)}</code></a>`;
      if (it && it.parent && items.has(it.parent)) return `<a href="${memberHref(lang, it)}"><code>${esc(label)}</code></a>`;
      return `<code>${esc(label)}</code>`;
    })
    .replace(/<xref href="([^"]+)"[^>]*>([^<]*)<\/xref>/g, (_, uid, t) => `<code>${esc(t)}</code>`);
}
function sourceLink(it) {
  const s = it.source;
  if (!s?.remote) return null;
  return `${s.remote.repo.replace(/\.git$/, "")}/blob/${s.remote.branch}/${s.remote.path}#L${(s.startLine ?? 0) + 1}`;
}
// "System.Threading.Tasks.ValueTask{McProtoNet.Primitives.IncomingPacket}" → ValueTask<IncomingPacket>, each known type linked
function typeDisplay(lang, uid) {
  const it = items.get(uid);
  if (it && TYPE_KINDS.includes(it.type)) return `<a href="${typeHref(lang, it.uid)}">${esc(it.name)}</a>`;
  const short = uid.replace(/\{/g, "<").replace(/\}/g, ">").replace(/`+\d+/g, "").replace(/\[\]/g, "[]")
    .replace(/[A-Za-z_]\w*(?:\.[A-Za-z_]\w*)+/g, m => m.split(".").pop());
  return linkifySig(lang, short);
}
// first sentence of an already-rendered summary; xrefs are resolved before cutting so "<xref>" never vanishes mid-sentence
const firstSentence = html => {
  const text = String(html ?? "").replace(/\s+/g, " ").trim();
  const m = text.match(/^.*?[.!?](?=\s|$)(?![^<]*>)/);
  return m ? m[0] : text;
};

// ---------- layout ----------
function sidebar(lang, active) {
  const L = LANGS[lang];
  let html = `<div class="grp">${L.groups.intro}</div>` + L.intro.map(([p, n]) => `<a class="${active === p ? "on" : ""}" href="/${lang}/${p}.html">${n}</a>`).join("");
  html += `<div class="grp">${L.groups.api}</div><a class="${active === "api" ? "on" : ""}" href="/${lang}/api/index.html">${L.apiIndexTitle}</a>`;
  const activeType = items.get(active);
  const activePkg = activeType ? pkgOf(activeType) : null, activeNs = activeType?.namespace;
  for (const pkg of packages) {
    const nss = tree.get(pkg);
    html += `<details class="pkg"${pkg === activePkg ? " open" : ""}><summary>${esc(pkg)}</summary>`;
    for (const ns of [...nss.keys()].sort()) {
      const list = nss.get(ns);
      const single = nss.size === 1;
      const inner = list.map(t => `<a class="${active === t.uid ? "on" : ""}" href="${typeHref(lang, t.uid)}" title="${esc(t.name)}">${esc(t.name)}</a>`).join("");
      html += single ? inner : `<details class="ns"${ns === activeNs ? " open" : ""}><summary title="${esc(ns)}">${esc(nsLabel(L, pkg, ns))}</summary>${inner}</details>`;
    }
    html += `</details>`;
  }
  return html;
}
function layout(lang, { title, crumbs, active, body, otherHref, toc }) {
  const L = LANGS[lang];
  const crumbHtml = crumbs.map((c, i) => i === crumbs.length - 1 ? `<b>${esc(c[0])}</b>` : `<a href="${c[1]}">${esc(c[0])}</a>`).join('<span class="sep">/</span>');
  const tocHtml = toc?.length ? `<aside class="toc"><div class="grp">${L.labels.onThisPage}</div>${toc.map(t => `<a class="l${t.level}" href="#${t.id}">${esc(t.text)}</a>`).join("")}</aside>` : `<aside class="toc"></aside>`;
  return `<!doctype html><html lang="${lang}"><head><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1">
<title>${esc(title)} — McProtoNet</title><link rel="stylesheet" href="/styles.css"><link rel="alternate" hreflang="${other(lang)}" href="${otherHref}"></head><body>
<header class="top"><div class="brand"><a href="/${lang}/index.html">McProtoNet</a><span class="ver">${VERSION}</span></div>
<div class="search"><input id="q" type="search" placeholder="${L.search}" autocomplete="off"><div id="hits" class="hits"></div></div>
<nav class="links"><a href="/${lang}/index.html">${L.nav.overview}</a><a href="https://www.nuget.org/packages/McProtoNet">${L.nav.nuget}</a><a href="https://github.com/Titlehhhh/McProtoNet">${L.nav.github}</a>
<span class="lang"><a class="${lang === "en" ? "on" : ""}" href="${lang === "en" ? "#" : otherHref}">EN</a><a class="${lang === "ru" ? "on" : ""}" href="${lang === "ru" ? "#" : otherHref}">RU</a></span></nav></header>
<div class="banner"><b>preview</b> ${L.banner}</div>
<div class="page"><aside class="side">${sidebar(lang, active)}</aside><main class="main"><div class="crumbs">${crumbHtml}</div>${body}</main>${tocHtml}</div>
<script>window.__lang=${JSON.stringify(lang)};</script><script src="/search.js"></script></body></html>`;
}

// ---------- API: type page ----------
function memberTable(lang, members) {
  // one row per overload group; the row links to the member page
  const seen = new Set(), rows = [];
  for (const m of members) {
    const k = overloadKey(m);
    if (seen.has(k)) continue;
    seen.add(k);
    const group = members.filter(x => overloadKey(x) === k);
    const sig = group.length > 1 ? `${shortSig(m).replace(/\(.*$/, "")}(…)  <span class="k">×${group.length}</span>` : linkifySig(lang, shortSig(m));
    rows.push(`<tr><td class="sig"><a href="${memberHref(lang, m)}">${group.length > 1 ? esc(shortSig(m).replace(/\(.*$/, "")) + `(…) <span class="k">×${group.length}</span>` : esc(shortSig(m))}</a></td><td class="sum">${firstSentence(xrefToHtml(lang, m.summary)) || LANGS[lang].labels.noSummary}</td></tr>`);
  }
  return `<table class="members">${rows.join("")}</table>`;
}
function defBlock(lang, it) {
  const L = LANGS[lang].labels;
  const pkg = pkgOf(it);
  const rows = [[L.namespace, `<a href="/${lang}/api/index.html#ns-${slug(it.namespace)}">${esc(it.namespace)}</a>`], [L.assembly, `<code>${esc(pkg)}.dll</code>`], [L.package, `<a href="https://www.nuget.org/packages/${esc(pkg)}">${esc(pkg)}</a>`]];
  const src = sourceLink(it);
  if (src) rows.push([L.source, `<a href="${src}">${esc(it.source.remote.path)}#L${(it.source.startLine ?? 0) + 1} ↗</a>`]);
  return `<table class="def">${rows.map(([k, v]) => `<tr><th>${k}</th><td>${v}</td></tr>`).join("")}</table>`;
}
function typePage(lang, it) {
  const L = LANGS[lang];
  const enNote = lang === "ru" ? `<p class="note">Описания из XML-комментариев — пока только на английском.</p>` : "";
  const kids = (it.children ?? []).map(u => items.get(u)).filter(Boolean);
  const by = t => kids.filter(k => k.type === t);
  const groups = [["ctor", by("Constructor")], ["prop", by("Property")], ["field", by("Field")], ["method", by("Method")], ["event", by("Event")]].filter(g => g[1].length);
  const toc = [{ id: "definition", level: 1, text: L.labels.definition }];
  let body = `<div class="kind">${esc(kindLabel(it))}</div><h1>${esc(it.name)}</h1>${enNote}<div class="lead">${xrefToHtml(lang, it.summary)}</div>`;
  body += `<h2 id="definition">${L.labels.definition}</h2>${defBlock(lang, it)}<pre class="decl"><code>${esc(it.syntax?.content ?? "")}</code></pre>`;
  if (it.inheritance?.length > 1) body += `<p class="meta">${L.labels.inherits}: ${it.inheritance.slice(0, -1).map(u => typeDisplay(lang, u)).join(" → ")} → <b>${esc(it.name)}</b></p>`;
  if (it.implements?.length) body += `<p class="meta">${L.labels.implements}: ${it.implements.map(u => typeDisplay(lang, u)).join(", ")}</p>`;
  if (it.derivedClasses?.length) body += `<p class="meta">${L.labels.derived}: ${it.derivedClasses.map(u => typeDisplay(lang, u)).join(", ")}</p>`;
  if (it.remarks) { toc.push({ id: "remarks", level: 1, text: L.labels.remarks }); body += `<h2 id="remarks">${L.labels.remarks}</h2><div class="remarks">${xrefToHtml(lang, it.remarks)}</div>`; }
  if (it.example?.length) { toc.push({ id: "examples", level: 1, text: L.labels.examples }); body += `<h2 id="examples">${L.labels.examples}</h2>${it.example.map(e => xrefToHtml(lang, e)).join("")}`; }
  for (const [k, list] of groups) {
    toc.push({ id: k, level: 1, text: L.sections[k] });
    body += `<h2 id="${k}">${L.sections[k]}</h2>${memberTable(lang, list)}`;
  }
  if (it.extensionMethods?.length) { toc.push({ id: "ext", level: 1, text: L.sections.ext }); body += `<h2 id="ext">${L.sections.ext}</h2><ul class="ext">${it.extensionMethods.map(u => `<li><code>${esc(u.split(".").slice(-1)[0])}</code></li>`).join("")}</ul>`; }
  return layout(lang, {
    title: it.name, active: it.uid, body, toc,
    crumbs: [[L.crumbs.docs, `/${lang}/index.html`], [L.crumbs.api, `/${lang}/api/index.html`], [pkgOf(it), `/${lang}/api/index.html#pkg-${slug(pkgOf(it))}`], [it.name]],
    otherHref: typeHref(other(lang), it.uid),
  });
}

// ---------- API: member page (one per overload group) ----------
function memberPage(lang, parent, group) {
  const L = LANGS[lang];
  const m0 = group[0];
  const title = m0.name.replace(/\(.*$/, "");
  const toc = [];
  let body = `<div class="kind">${esc(parent.name)}.${esc(title)}</div><h1>${esc(title)}</h1><div class="lead">${xrefToHtml(lang, m0.summary)}</div>`;
  body += `<table class="def"><tr><th>${L.labels.declaringType}</th><td><a href="${typeHref(lang, parent.uid)}">${esc(parent.name)}</a></td></tr><tr><th>${L.labels.namespace}</th><td>${esc(parent.namespace)}</td></tr><tr><th>${L.labels.assembly}</th><td><code>${esc(pkgOf(parent))}.dll</code></td></tr></table>`;
  if (group.length > 1) {
    toc.push({ id: "overloads", level: 1, text: L.labels.overloads });
    body += `<h2 id="overloads">${L.labels.overloads}</h2><table class="members">${group.map(m => `<tr><td class="sig"><a href="#${slug(m.uid)}">${esc(shortSig(m))}</a></td><td class="sum">${firstSentence(xrefToHtml(lang, m.summary)) || L.labels.noSummary}</td></tr>`).join("")}</table>`;
  }
  for (const m of group) {
    const p = m.syntax?.parameters ?? [], ret = m.syntax?.return, src = sourceLink(m);
    const id = slug(m.uid);
    if (group.length > 1) { toc.push({ id, level: 1, text: shortSig(m).replace(/^.*?\s(?=[A-Za-z_][A-Za-z0-9_]*\s*\()/, "") }); body += `<h2 id="${id}"><code>${linkifySig(lang, shortSig(m))}</code></h2>`; }
    else body += `<h2 id="${id}">${L.labels.definition}</h2>`;
    if (group.length > 1 && m.summary !== m0.summary) body += `<p>${xrefToHtml(lang, m.summary)}</p>`;
    body += `<pre class="decl"><code>${esc(m.syntax?.content ?? "")}</code></pre>`;
    if (src) body += `<p><a class="src" href="${src}">${L.labels.source} ↗</a></p>`;
    if (p.length) body += `<div class="lbl">${L.labels.params}</div><table class="params">${p.map(x => `<tr><td><code>${esc(x.id)}</code></td><td class="t">${typeDisplay(lang, x.type)}</td><td>${xrefToHtml(lang, x.description)}</td></tr>`).join("")}</table>`;
    if (ret && ret.type && ret.type !== "System.Void") body += `<div class="lbl">${L.labels.returns}</div><p><span class="t">${typeDisplay(lang, ret.type)}</span> ${xrefToHtml(lang, ret.description)}</p>`;
    if (m.exceptions?.length) body += `<div class="lbl">${L.labels.exceptions}</div><table class="params">${m.exceptions.map(x => `<tr><td class="t">${typeDisplay(lang, x.type)}</td><td>${xrefToHtml(lang, x.description)}</td></tr>`).join("")}</table>`;
    if (m.remarks) { if (group.length === 1) toc.push({ id: "remarks", level: 1, text: L.labels.remarks }); body += `<h3 id="remarks">${L.labels.remarks}</h3><div class="remarks">${xrefToHtml(lang, m.remarks)}</div>`; }
    if (m.example?.length) body += `<h3>${L.labels.examples}</h3>${m.example.map(e => xrefToHtml(lang, e)).join("")}`;
  }
  return layout(lang, {
    title: `${parent.name}.${title}`, active: parent.uid, body, toc,
    crumbs: [[L.crumbs.docs, `/${lang}/index.html`], [L.crumbs.api, `/${lang}/api/index.html`], [pkgOf(parent), `/${lang}/api/index.html#pkg-${slug(pkgOf(parent))}`], [parent.name, typeHref(lang, parent.uid)], [title]],
    otherHref: memberHref(other(lang), m0),
  });
}

// ---------- API: index ----------
function apiIndex(lang) {
  const L = LANGS[lang];
  const toc = [];
  let body = `<h1>${L.apiIndexTitle}</h1><div class="lead">${L.apiIndexLead}</div>`;
  for (const pkg of packages) {
    toc.push({ id: "pkg-" + slug(pkg), level: 1, text: pkg });
    body += `<h2 id="pkg-${slug(pkg)}">${esc(pkg)}</h2>`;
    const nss = tree.get(pkg);
    for (const ns of [...nss.keys()].sort()) {
      if (nss.size > 1) { toc.push({ id: "ns-" + slug(ns), level: 2, text: nsLabel(L, pkg, ns) }); body += `<h3 id="ns-${slug(ns)}">${esc(ns)}</h3>`; }
      else body += `<a id="ns-${slug(ns)}"></a>`;
      body += `<table class="members">${nss.get(ns).map(t => `<tr><td class="sig"><span class="k">${esc(kindLabel(t))}</span> <a href="${typeHref(lang, t.uid)}">${esc(t.name)}</a></td><td class="sum">${firstSentence(xrefToHtml(lang, t.summary)) || L.labels.noSummary}</td></tr>`).join("")}</table>`;
    }
  }
  return layout(lang, { title: L.apiIndexTitle, active: "api", body, toc, crumbs: [[L.crumbs.docs, `/${lang}/index.html`], [L.crumbs.api]], otherHref: `/${other(lang)}/api/index.html` });
}

// ---------- markdown pages ----------
function mdPage(lang, name) {
  const L = LANGS[lang];
  const md = fs.readFileSync(path.join(CONTENT, lang, `${name}.md`), "utf8");
  const title = (md.match(/^#\s+(.+)$/m) ?? [, name])[1];
  const toc = [];
  const renderer = new marked.Renderer();
  renderer.heading = ({ tokens, depth }) => {
    const text = tokens.map(t => t.raw ?? t.text ?? "").join("");
    const id = text.toLowerCase().replace(/[^\p{L}\p{N}]+/gu, "-").replace(/^-|-$/g, "");
    if (depth === 2 || depth === 3) toc.push({ id, level: depth - 1, text });
    return `<h${depth} id="${id}">${marked.parseInline(text)}</h${depth}>`;
  };
  const html = marked.parse(md.replace(/\(xref:McProtoNet\)/g, `(/${lang}/api/index.html)`).replace(/\]\(([\w-]+)\.md\)/g, `](/${lang}/$1.html)`), { renderer });
  return layout(lang, { title, active: name, body: `<article class="doc">${html}</article>`, toc, crumbs: [[L.crumbs.docs, `/${lang}/index.html`], [title]], otherHref: `/${other(lang)}/${name}.html` });
}

// ---------- write ----------
fs.mkdirSync(OUT, { recursive: true });
fs.copyFileSync(path.join(ROOT, "styles.css"), path.join(OUT, "styles.css"));
fs.copyFileSync(path.join(ROOT, "search.js"), path.join(OUT, "search.js"));
fs.writeFileSync(path.join(OUT, "index.html"), `<!doctype html><meta charset="utf-8"><script>location.replace((navigator.language||"en").toLowerCase().startsWith("ru")?"/ru/index.html":"/en/index.html")</script><meta http-equiv="refresh" content="1; url=/en/index.html">`);
let pages = 0;
for (const lang of Object.keys(LANGS)) {
  fs.mkdirSync(path.join(OUT, lang, "api"), { recursive: true });
  for (const [p] of LANGS[lang].intro) { fs.writeFileSync(path.join(OUT, lang, `${p}.html`), mdPage(lang, p)); pages++; }
  fs.writeFileSync(path.join(OUT, lang, "api", "index.html"), apiIndex(lang)); pages++;
  for (const t of types) {
    fs.writeFileSync(path.join(OUT, lang, "api", `${slug(t.uid)}.html`), typePage(lang, t)); pages++;
    const kids = (t.children ?? []).map(u => items.get(u)).filter(k => k && !TYPE_KINDS.includes(k.type));
    const groups = new Map();
    for (const k of kids) { const key = overloadKey(k); if (!groups.has(key)) groups.set(key, []); groups.get(key).push(k); }
    for (const [key, g] of groups) { fs.writeFileSync(path.join(OUT, lang, "api", `${slug(key)}.html`), memberPage(lang, t, g)); pages++; }
  }
}
const strip = h => String(h).replace(/<[^>]+>/g, "").replace(/&amp;/g, "&").replace(/&lt;/g, "<").replace(/&gt;/g, ">");
const searchIndex = [];
for (const t of types) {
  searchIndex.push({ n: t.name, ns: t.namespace, u: slug(t.uid), s: strip(firstSentence(xrefToHtml("en", t.summary))).slice(0, 120) });
  const seen = new Set();
  for (const u of t.children ?? []) {
    const m = items.get(u); if (!m || TYPE_KINDS.includes(m.type)) continue;
    const key = overloadKey(m); if (seen.has(key)) continue; seen.add(key);
    searchIndex.push({ n: `${t.name}.${m.name.replace(/\(.*$/, "")}`, ns: t.namespace, u: slug(key), s: strip(firstSentence(xrefToHtml("en", m.summary))).slice(0, 120), m: 1 });
  }
}
fs.writeFileSync(path.join(OUT, "search-index.json"), JSON.stringify(searchIndex));
console.log(`types: ${types.length}, packages: ${packages.length}, pages: ${pages} → ${OUT}`);
