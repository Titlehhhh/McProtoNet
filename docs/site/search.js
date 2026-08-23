(() => {
  const lang = window.__lang || "en";
  const T = lang === "ru" ? { none: "Ничего не найдено", hint: "Ctrl+K" } : { none: "Nothing found", hint: "Ctrl+K" };

  // ---- search ----
  const q = document.getElementById("q"), hits = document.getElementById("hits");
  if (q) {
    let idx = null, sel = -1, cur = [];
    const load = async () => idx ??= await (await fetch("/search-index.json")).json();
    const render = () => {
      if (!q.value.trim()) { hits.style.display = "none"; return; }
      hits.innerHTML = cur.length
        ? cur.map((t, i) => `<a role="option" aria-selected="${i === sel}" class="${i === sel ? "sel" : ""}${t.m ? " mem" : ""}" href="/${lang}/api/${t.u}.html"><span class="n">${t.n}</span><span class="ns">${t.ns}</span>${t.s ? `<span class="s">${t.s}</span>` : ""}</a>`).join("")
        : `<div class="none">${T.none}</div>`;
      hits.style.display = "block";
    };
    q.addEventListener("input", async () => {
      const s = q.value.trim().toLowerCase();
      sel = -1;
      if (!s) { cur = []; render(); return; }
      const all = await load();
      const score = t => {
        const n = t.n.toLowerCase(), short = n.split(".").pop();
        if (short === s) return 0; if (short.startsWith(s)) return 1; if (n.includes(s)) return 2; if (t.s.toLowerCase().includes(s)) return 3; return 9;
      };
      cur = all.map(t => [score(t), t]).filter(x => x[0] < 9).sort((a, b) => a[0] - b[0] || (a[1].m || 0) - (b[1].m || 0)).slice(0, 14).map(x => x[1]);
      render();
    });
    q.addEventListener("keydown", e => {
      if (e.key === "Escape") { q.value = ""; cur = []; render(); q.blur(); }
      else if (e.key === "ArrowDown") { e.preventDefault(); sel = Math.min(sel + 1, cur.length - 1); render(); }
      else if (e.key === "ArrowUp") { e.preventDefault(); sel = Math.max(sel - 1, 0); render(); }
      else if (e.key === "Enter" && cur.length) { location.href = `/${lang}/api/${cur[Math.max(sel, 0)].u}.html`; }
    });
    document.addEventListener("keydown", e => { if ((e.ctrlKey || e.metaKey) && e.key.toLowerCase() === "k") { e.preventDefault(); q.focus(); q.select(); } });
    document.addEventListener("click", e => { if (!e.target.closest(".search")) hits.style.display = "none"; });
    q.addEventListener("focus", () => { if (q.value.trim()) render(); });
    q.setAttribute("aria-label", q.placeholder);
    q.placeholder += `  ·  ${T.hint}`;
  }

  // ---- "in this article": highlight the section in view ----
  const toc = document.querySelector(".toc");
  if (toc) {
    const links = [...toc.querySelectorAll("a[href^='#']")];
    const targets = links.map(a => document.getElementById(decodeURIComponent(a.getAttribute("href").slice(1)))).filter(Boolean);
    if (targets.length) {
      const set = id => links.forEach(a => a.classList.toggle("on", a.getAttribute("href") === "#" + id));
      const io = new IntersectionObserver(entries => {
        const vis = entries.filter(e => e.isIntersecting).sort((a, b) => a.boundingClientRect.top - b.boundingClientRect.top);
        if (vis.length) set(vis[0].target.id);
      }, { rootMargin: "-10% 0px -70% 0px", threshold: 0 });
      targets.forEach(t => io.observe(t));
      set(targets[0].id);
    }
  }

  // ---- keep the active sidebar entry in view ----
  const on = document.querySelector(".side a.on");
  if (on) on.scrollIntoView({ block: "center" });
})();
