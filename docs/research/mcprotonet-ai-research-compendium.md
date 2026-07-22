# McProtoNet AI research compendium

Дата сборки: 2026-06-08.

Этот файл заменяет россыпь предыдущих research-файлов как единая рабочая карта. Старые файлы использованы как сырье: из них перенесены факты, проекты, архитектурные идеи, риски и вопросы, но документ не является индексом ссылок на них.

## Краткое оглавление

1. [Зачем этот документ](#зачем-этот-документ)
2. [Карта найденных проектов и технологий по категориям](#карта-найденных-проектов-и-технологий-по-категориям)
3. [Большая таблица проектов и технологий](#большая-таблица-проектов-и-технологий)
4. [Локальный и полулокальный research-бот на день](#локальный-и-полулокальный-research-бот-на-день)
5. [Проблема уверенности ИИ и недопонимания](#проблема-уверенности-ии-и-недопонимания)
6. [Как объяснять тонкости McProtoNet ИИ](#как-объяснять-тонкости-mcprotonet-ии)
7. [Радикальные развилки](#радикальные-развилки)
8. [Что стоит проверить руками](#что-стоит-проверить-руками)
9. [Источники](#источники)

## Зачем этот документ

У McProtoNet уже накопилась не одна тема, а целый клубок:

- .NET runtime-библиотека для Minecraft Java Edition protocol;
- большой набор packet-классов и version ranges;
- `McProtoNet.SourceGenerator`;
- сериализация, NBT, performance-sensitive код;
- Writerside-документация;
- старый PacketGenerator с `minecraft-data`, protodef, MCP/REST, LLM generation, artifacts и complexity scoring;
- желание дать ИИ не "весь репозиторий на удачу", а точные карты, индексы, схемы, tests, traces и процедуры.

Предыдущие research-файлы полезны, но они стали отдельными островами: Agent Skills, MCP, документация, HeadlessMC, Obsidian, workspace architecture, technology radar. Этот compendium делает из них один документ: больше фактов и проектов, меньше прямых советов. Идея не в том, чтобы выбрать архитектуру вместо автора McProtoNet, а в том, чтобы дать максимум материала для собственных решений.

Важная смена угла: AI-слой для McProtoNet стоит думать не как "умный агент, который понял проект", а как набор проверяемых каналов знания:

- постоянный контекст: `AGENTS.md`, `llms.txt`, короткие canonical docs;
- процедурный контекст: Agent Skills в формате `SKILL.md`;
- живые ресурсы: MCP resources/tools/prompts;
- machine-readable факты: JSON Schema, packet catalogs, generated manifests;
- evidence layer: golden tests, protocol traces, CI artifacts, real server/client integration;
- visual layer: Mermaid, Excalidraw, Canvas, Obsidian/Dataview.

## Карта найденных проектов и технологий по категориям

### 1. Agent Skills ecosystem

**Agent Skills / `SKILL.md`.** Формат директории со `SKILL.md` и опциональными `scripts/`, `references/`, `assets/`. Важная идея: progressive disclosure. Агент сначала видит `name` и `description`, затем читает полный skill, затем открывает supporting files только при необходимости. Для McProtoNet это похоже на процедурную память: "как добавить packet", "как обновить protocol version", "как проверять NBT", "как ревьюить source generator".

**Vercel skills CLI и `vercel-labs/agent-skills`.** Vercel развивает skills как переносимые пакеты, которые ставятся в разные agent environments. Интересна не только коллекция, но и модель распространения: skill может быть developer-experience артефактом, почти как маленький handbook package.

**Anthropic skills repository.** Публичный reference corpus skills, показывающий, что skill может быть не только prompt snippet, а процедура с reference files и scripts.

**SkillsMD, skills.sh, Skillhound, awesome-agent-skills, Skilldex.** Это слой discovery/registry/package management. Полезная мысль для McProtoNet: если skills начинают устанавливаться извне, они становятся supply-chain поверхностью, а не просто Markdown.

**Security вокруг skills.** Исследования про malicious `SKILL.md`, semantic supply-chain attacks и prompt injection показывают, что skills надо проверять как зависимости. Для McProtoNet особенно важны scripts, network access, LLM keys, генерация кода и скрытые side effects.

### 2. MCP ecosystem

**MCP specification.** Model Context Protocol описывает host/client/server, JSON-RPC 2.0, lifecycle/capability negotiation, server primitives `tools`, `resources`, `prompts`, client primitives вроде sampling/elicitation/logging, transports stdio и Streamable HTTP. Для McProtoNet это не "чат-бот", а протокольный workbench.

**MCP C# SDK.** Официальный .NET SDK позволяет сделать сервер прямо на C#, рядом с reflection, Roslyn, source generator и packet metadata. Это снижает смысл выносить McProtoNet MCP в Node/Python только ради протокола.

**Registries и gateways.** Official MCP Registry, Docker MCP Gateway, Glama, Smithery и похожие проекты показывают, что MCP быстро становится инфраструктурой: discovery, policy, OAuth, audit, gateway, hosted servers.

**Security MCP.** Tool poisoning, prompt/resource injection, DNS rebinding, SSRF, OAuth discovery risks, session hijacking, environment leakage. Для McProtoNet безопасный первый слой выглядит как local stdio read-only resources плюс pure local validation tools.

### 3. Docs/context/code graph

**`AGENTS.md`.** Простой Markdown entrypoint для coding agents. Хорош для always-on фактов: layout repo, build/test команды, sensitive areas, "не выводить packet IDs из памяти", "не коммитить без запроса".

**`llms.txt` и `llms-full.txt`.** Идея web-документации для LLM: краткий индекс и полный markdown corpus. Для McProtoNet может быть не только сайтом, но и generated repo artifact: "вот стабильная карта проекта для агентов".

**Context7.** Сервис, который дает агентам актуальные docs библиотек. Важен как паттерн: агенту нужен curated doc context, а не случайный web search.

**DeepWiki, OpenDeepWiki, RepoMind, RepoWiki-like tools.** Генерируют repo-level explanation, architecture views, Mermaid diagrams, chat по GitHub repo. Для McProtoNet это хороший внешний benchmark: что такая система поймет сама, а где ей нужны packet catalogs, traces и generated docs.

**Sourcegraph/OpenCtx, code graph/indexing.** Подход "код как граф и индекс", а не как набор файлов. Для McProtoNet может дать cross-reference между packets, attributes, generated helper code, tests, docs и version ranges.

### 4. Agentic codegen, validation и evals

**Structured Outputs и JSON Schema.** Вместо "напиши packet" агент сначала может выдать строго валидируемый JSON: packet name, state, direction, protocol range, IDs, fields, source refs, uncertainty, missing facts. Это промежуточный слой между разговором и C#.

**Typed IR.** Старый PacketGenerator уже намекал на отдельный protocol IR: `minecraft-data`/protodef loader, complexity scoring, artifacts, generated code. Большая развилка: C# attributes как source of truth или отдельный schema-first IR, из которого генерируются C#, docs, tests и MCP resources.

**Golden tests и validation loops.** Проверять не только код, но и agent workflow: умеет ли агент найти правильный packet ID, спросить уточнение, отказаться от догадки, выдать diff, не трогать unrelated files.

**Self-improving skills.** Идея привлекательная: агент после ошибки предлагает обновить skill. Риск тоже очевиден: skill может закрепить ложное правило. Для McProtoNet self-improvement безопасен только через human review и golden tests.

**SWE-Skills-Bench, SkillSmith, Trace2Skill-подобные идеи.** Общее направление: превращать успешные traces в reusable skills и оценивать, помогают ли они в software engineering tasks.

### 5. Minecraft/protocol ecosystem

**PrismarineJS `minecraft-data`.** Мультиверсионный dataset по protocol data. Важен как machine-readable oracle и как пример, что ценным продуктом может быть не только библиотека, но и versioned corpus.

**`node-minecraft-protocol`, mineflayer, flying-squid.** Packet parsing/serialization, client/server, bot API, fake server. Полезны для differential tests и сценариев "McProtoNet bytes -> Prismarine decode" и наоборот.

**MCProtocolLib.** Java protocol library от GeyserMC ecosystem. Хороший независимый oracle, особенно для login/encryption/compression/session behavior.

**HeadlessMC.** CLI launcher для Minecraft Java client без GUI. Полезен как e2e слой: настоящий клиент подключается к McProtoNet fake server или используется рядом с real server.

**itzg Docker Minecraft Server и Testcontainers for .NET.** Практичный слой real server integration: Vanilla/Paper/Fabric/Forge containers, EULA, offline mode, wait strategies, logs as artifacts.

**ViaVersion/ViaBackwards/ViaRewind/ViaLegacy/ViaProxy.** Мультиверсия как graph of translations. Это другая архитектурная философия: не "держать все packet classes", а строить transformations между версиями.

**ProtocolLib и PacketEvents.** Plugin-facing packet abstraction. Важны не как абсолютный corpus, а как UX: wrappers, version predicates, clear unsupported behavior.

**Minestom, PicoLimbo, go-mc, pyCraft, Pumpkin, Valence, Azalea.** Показывают разные стратегии: latest-first, subset packets, tags per Minecraft version, limbo vertical slice, bot semantic layer.

### 6. Visual/knowledge workflows

**Obsidian.** Репозиторий можно открыть как vault: research notes, backlinks, local graph, links на `src/` и `docs/`.

**Mermaid.** Каноничные review-friendly схемы: state diagrams, sequence diagrams, class/architecture diagrams, packet flow.

**Excalidraw.** Исследовательские доски и rough sketches. В docs лучше публиковать SVG/PNG export, а source хранить отдельно.

**JSON Canvas.** Карта связей между notes/code/docs/cards. Хороша для thinking board, но не как основной publishable docs format.

**Dataview.** Индексы по frontmatter: research notes, diagrams, decisions, artifacts. Не должен быть единственным источником истины, потому что это Obsidian-specific runtime.

### 7. Research-боты, web agents и GitHub mining

**Local Deep Researcher.** LangChain проект для локального web research и report writing, с Ollama/LM Studio и search backends вроде SearXNG, Tavily, Perplexity.

**Open Deep Research.** LangChain fully open-source deep research agent, configurable across model providers, search tools and MCP servers.

**OpenDeepResearcher via SearXNG.** Fork/вариант, который завязывает OpenDeepResearcher на OpenAI-compatible endpoint, local Playwright и SearXNG.

**browser-use.** Python library/framework для browser automation агентами на Playwright.

**BrowserOS.** Open-source agentic browser с built-in tools, MCP integrations, scheduled tasks, local memory и BYO LLM.

**AgenticSeek.** Local-first voice-enabled assistant, который обещает web browsing, coding и planning локально.

**Firecrawl web-agent.** Open-source foundation для structured web research: Search, Scrape, Interact, Skills, Subagents, structured output.

**Rival Search MCP.** Search MCP server с deterministic tools и источниками web/social/news/academic/code/documents, без LLM внутри сервера.

**TinySearch.** Маленький local MCP/FastAPI research layer с DuckDuckGo, Crawl4AI, dense embeddings, BM25/reranking и ограниченным context output.

**GitTrend, OSSInsight, RepoRank, GitRepoTrend, GitHub API mining.** Инструменты и подходы для поиска свежих проектов: trending repos, GitHub event data, rankings по engagement/recency, SQL-like analysis, scheduled mining.

## Большая таблица проектов и технологий

| Проект / технология | Что это | Что дает | Почему может быть интересно для McProtoNet / ИИ-объяснения проекта | Ссылки |
| --- | --- | --- | --- | --- |
| `AGENTS.md` | Markdown-инструкции для coding agents | Always-on контекст: layout, команды, правила | Базовый слой, который агент должен видеть до skills и MCP | <https://agents.md/>, <https://developers.openai.com/codex/guides/agents-md> |
| Agent Skills / `SKILL.md` | Формат переносимых процедур для агентов | Progressive disclosure, scripts, references, assets | Packet authoring, protocol update, NBT, performance, source generator review | <https://agentskills.io/specification> |
| Vercel `skills` CLI | CLI для установки agent skills | Skills как пакеты, project/global scopes | Возможность McProtoNet skill-pack для контрибьюторов | <https://github.com/vercel-labs/skills> |
| Vercel `agent-skills` | Коллекция skills от Vercel | Реальные примеры structured skills | Форма для `mcprotonet-*` skills | <https://github.com/vercel-labs/agent-skills> |
| Anthropic skills | Публичный reference repo | Показывает layout skills и supporting files | Можно сравнить, насколько McProtoNet skills компактны | <https://github.com/anthropics/skills> |
| SkillsMD | Registry/marketplace markdown skills | Discovery и установка skills | Поднимает supply-chain вопросы | <https://skillsmd.dev/>, <https://skillsmd.co/> |
| skills.sh | Directory/leaderboard для skills | Поиск популярных skills | Можно мониторить появление полезных research/code skills | <https://www.skills.sh/> |
| Skillhound | Индексация `SKILL.md` на GitHub | Поиск реальных skills в wild | Источник примеров и anti-patterns | <https://www.skillhound.ai/> |
| Awesome Agent Skills | Каталоги skill packs | Карта ecosystem | Полезно смотреть на naming, triggers, scopes | <https://github.com/skillcreatorai/Awesome-Agent-Skills> |
| Skilldex | Идея package manager/registry для skills | Namespace, scopes, MCP registry | Модель будущего "NuGet для skills" | <https://arxiv.org/abs/2604.16911> |
| SkillAttack / SkillSieve | Исследования security skills | Threat model для `SKILL.md` | Нельзя бездумно брать чужие skills с scripts | <https://arxiv.org/abs/2604.04989>, <https://arxiv.org/abs/2604.06550> |
| Model Context Protocol | Протокол tools/resources/prompts | Единый слой для agent tools/data | McProtoNet MCP как protocol workbench | <https://modelcontextprotocol.io/docs/learn/architecture> |
| MCP C# SDK | Официальный .NET SDK | Stdio/HTTP server на C# | Использовать reflection/Roslyn/source generators прямо в MCP | <https://github.com/modelcontextprotocol/csharp-sdk> |
| MCP Registry | Registry публичных MCP servers | Discovery и metadata | Если McProtoNet MCP станет публичным catalog server | <https://modelcontextprotocol.io/registry/about> |
| Docker MCP Gateway | Gateway для MCP servers | Policy, запуск, инфраструктура | Оболочка перед опасными generation tools | <https://docs.docker.com/reference/cli/docker/mcp/gateway/gateway_run/> |
| Glama MCP Gateway | Registry/gateway/analytics | Tool access control, OAuth, logs | Паттерн audit/policy для agent tools | <https://glama.ai/> |
| MCP security best practices | Рекомендации безопасности MCP | Origin validation, auth, input validation | Нужны для remote McProtoNet MCP | <https://modelcontextprotocol.io/docs/tutorials/security/security_best_practices> |
| OWASP MCP Tool Poisoning | Threat description | Tool descriptions как attack surface | Tools McProtoNet должны быть короткими и проверяемыми | <https://owasp.org/www-community/attacks/MCP_Tool_Poisoning> |
| `llms.txt` | Предложение для LLM-readable docs | Краткий индекс документации | Generated `llms.txt` по McProtoNet docs/API/packets | <https://llmstxt.org/> |
| Context7 | Актуальные docs для agents | Curated doc context | Паттерн "не web search, а правильный context source" | <https://context7.com/docs> |
| DeepWiki | AI docs/chat для GitHub repos | Repo wiki без ручной подготовки | Проверить, что внешний AI поймет в McProtoNet сам | <https://deepwiki.org/> |
| OpenDeepWiki / RepoWiki-like | Self-hosted DeepWiki alternatives | Локальная repo wiki | Можно генерировать собственную wiki по McProtoNet | <https://github.com/topics/deepwiki> |
| RepoMind | GitHub repo analysis/chat | Architecture views, Mermaid, repo context | Аналог "DeepWiki для архитектурной карты" | <https://repomind.in/about> |
| Sourcegraph / OpenCtx | Code search/context architecture | Code graph, external context providers | Связать packets, docs, tests, generated artifacts | <https://sourcegraph.com/blog/anatomy-of-a-coding-assistant> |
| DocFX | .NET API docs из assemblies/XML | API reference | Генерировать API docs, не писать вручную | <https://dotnet.github.io/docfx/docs/dotnet-api-docs.html> |
| Writerside | JetBrains docs tool | Human-facing docs, code samples, Mermaid | Уже есть в McProtoNet, можно усилить generated docs | <https://www.jetbrains.com/help/writerside/code.html> |
| Docusaurus | MDX/React docs site | Interactive docs | Развилка, если нужен React/MDX public site | <https://docusaurus.io/docs/markdown-features> |
| Starlight | Astro docs toolkit | Markdown-first docs + content collections | Развилка для typed packet docs | <https://starlight.astro.build/guides/authoring-content/> |
| Structured Outputs | Строгий JSON output от LLM | Валидируемый промежуточный слой | Packet proposal before code edits | <https://platform.openai.com/docs/guides/structured-outputs> |
| JSON Schema | Схемы данных | Validation contracts | Packet schema, tool input/output, generated catalogs | <https://json-schema.org/> |
| OpenAI Evals | Evals framework/API | Проверка agent behavior | Evals на "не угадывай packet ID", "спроси уточнение" | <https://platform.openai.com/docs/api-reference/evals> |
| SWE-bench | Benchmark software engineering tasks | Измерение agent coding | Идея benchmark для McProtoNet tasks | <https://www.swebench.com/> |
| SWE-Skills-Bench | Benchmark роли skills в SWE | Мерить пользу skills | Проверять McProtoNet skills на реальных задачах | <https://arxiv.org/abs/2603.15401> |
| Local Deep Researcher | Локальный research/report agent | Ollama/LM Studio + search | Можно оставить на день собирать проекты и источники | <https://github.com/langchain-ai/local-deep-researcher> |
| Open Deep Research | Open-source deep research agent | Configurable models/search/MCP | Полулокальный research pipeline с сильной моделью | <https://github.com/langchain-ai/open_deep_research> |
| OpenDeepResearcher via SearXNG | OpenDeepResearcher + SearXNG/local endpoint | Local-ish web research | Вариант без платного search API | <https://github.com/benhaotang/OpenDeepResearcher-via-searxng> |
| SearXNG | Self-hosted metasearch | Private metasearch aggregator | Search backend для local research bots | <https://docs.searxng.org/user/about.html> |
| browser-use | Browser automation framework | Агент кликает/читает web через Playwright | Проверять docs, GitHub, generated sites, web research | <https://github.com/browser-use/browser-use> |
| BrowserOS | Open-source agentic browser | Browser tools, MCP integrations, scheduled tasks | Долгие web workflows и "оставить на день" | <https://www.browseros.com/>, <https://github.com/browseros-ai/BrowserOS> |
| AgenticSeek | Local-first AI assistant | Web browsing, coding, planning локально | Эксперимент с private local agent | <https://github.com/andrewstack-maker/agenticSeek> |
| Firecrawl web-agent | Structured web research agent foundation | Search/Scrape/Interact, Skills, Subagents | Хорош для clean context extraction и research reports | <https://github.com/firecrawl/web-agent> |
| Rival Search MCP | Search MCP server | Web/social/news/academic/code/doc tools | Подключить агенту deterministic search без LLM внутри server | <https://rivalsearchmcp.com/> |
| TinySearch | Маленький local MCP research tool | DuckDuckGo, Crawl4AI, embeddings, BM25 | Хорош для малых локальных моделей: мало, но чисто | <https://github.com/MarcellM01/TinySearch> |
| GitTrend | Trending GitHub repositories | Daily engagement tracking | Искать новые protocol/codegen/agent projects | <https://gittrend.io/> |
| OSSInsight | GitHub event analytics | SQL/AI over 10B+ GitHub events | Mining проектов по темам Minecraft/protocol/MCP | <https://ossinsight.io/>, <https://ossinsight.io/explore/> |
| RepoRank / GitRepoTrend | Repo ranking по engagement/recency | Альтернатива raw GitHub Trending | Фильтр шума для "оставить на день" research | <https://reporank.co/>, <https://gitrepotrend.com/> |
| GitHub API mining | Прямой поиск repos/issues/commits | Свой crawler/ranker | Ночная задача: новые проекты по topics, stars, updates | <https://docs.github.com/en/rest> |
| PrismarineJS `minecraft-data` | Versioned Minecraft data corpus | Packet IDs, protocol schemas, data | External oracle и possible input для IR | <https://github.com/PrismarineJS/minecraft-data> |
| `node-minecraft-protocol` | JS protocol client/server | Parse/serialize packets | Differential tests против McProtoNet | <https://github.com/PrismarineJS/node-minecraft-protocol> |
| mineflayer | High-level Minecraft bot | Semantic bot layer | Показать разницу codec vs bot semantics | <https://github.com/PrismarineJS/mineflayer> |
| MCProtocolLib | Java Minecraft protocol library | Независимый Java oracle | Проверка login/encryption/compression flows | <https://github.com/GeyserMC/MCProtocolLib> |
| HeadlessMC | Headless Minecraft client launcher | Настоящий Java client без GUI | E2E "real client accepts McProtoNet server" | <https://github.com/headlesshq/headlessmc> |
| itzg Minecraft Server | Docker image Minecraft server | Vanilla/Paper/Fabric/Forge CI servers | Real server integration tests | <https://github.com/itzg/docker-minecraft-server> |
| Testcontainers for .NET | Docker containers from tests | Управляемый lifecycle и wait strategies | Integration tests с real server | <https://dotnet.testcontainers.org/> |
| ViaVersion | Protocol translation plugin | New clients on old servers | Мультиверсия как graph of transformations | <https://viaversion.com/> |
| ViaBackwards | Old clients on new servers | Reverse compatibility | Подумать о normalized model vs per-version classes | <https://github.com/ViaVersion/ViaBackwards> |
| ViaRewind / ViaLegacy | Legacy protocol support | 1.7/1.8 и древние версии | Уроки extreme version history | <https://github.com/ViaVersion/ViaRewind>, <https://github.com/ViaVersion/ViaLegacy> |
| ViaProxy | Standalone proxy | Translation без server plugin | Trace трансляций как oracle для diff model | <https://github.com/ViaVersion/ViaProxy> |
| PacketEvents | Packet API для plugins/platforms | Wrappers, version checks | UX для public packet API | <https://docs.packetevents.com/> |
| ProtocolLib | Bukkit/Spigot/Paper packet middleware | Interception API | Стабильный API поверх unstable NMS | <https://protocollib.org/> |
| Minestom | Lightweight Java server framework | Controlled server behavior | Fake-ish integration server | <https://github.com/Minestom/Minestom> |
| PicoLimbo | Multi-version limbo server | Узкий сценарий, широкий range | Thin vertical slice как тестовая стратегия | <https://github.com/Quozul/PicoLimbo> |
| pyCraft | Python Minecraft client library | Compatible subset, честная неполнота | "Version support" не равно "all packets" | <https://github.com/ammaraskar/pyCraft> |
| go-mc | Go libraries for Minecraft | Version tags per MC release | Альтернативная версия package strategy | <https://github.com/Tnze/go-mc> |
| Obsidian | Markdown vault | Backlinks, graph, research notes | Репозиторий как knowledge vault | <https://obsidian.md/help/data-storage> |
| Mermaid | Text diagrams | Review-friendly diagrams | State machine, login flow, packet pipeline | <https://mermaid.js.org/intro/syntax-reference.html> |
| Excalidraw Obsidian plugin | Drawings as Markdown + exports | Исследовательские схемы | Черновики архитектуры и protocol maps | <https://github.com/zsviczian/obsidian-excalidraw-plugin> |
| JSON Canvas | Spec for canvas boards | Cards/nodes/edges | Карты идей, не финальная docs | <https://jsoncanvas.org/spec/1.0/> |
| Dataview | Obsidian query engine | Индексы по frontmatter | Индекс research notes/diagrams/decisions | <https://blacksmithgu.github.io/obsidian-dataview/> |
| Ollama | Local model runner | Простая установка локальных моделей | Local summarizer/classifier/research worker | <https://ollama.com/> |
| LM Studio | GUI + local OpenAI-compatible server | Model browser, GPU offload | Удобно на Windows с GTX 1080 Ti | <https://lmstudio.ai/docs/app/system-requirements/> |
| llama.cpp | GGUF inference engine | CUDA/Vulkan/CPU, quantization | Базовый runtime для локальных моделей | <https://github.com/ggml-org/llama.cpp> |

## Локальный и полулокальный research-бот на день

Цель такого бота: не "решить архитектуру", а собрать больше материала: новые проекты, похожие библиотеки, MCP servers, examples, issue discussions, benchmarks, свежие papers, forks и anti-patterns. Хорошая дневная задача для него выглядит так:

- найти 50-100 GitHub projects по темам `minecraft protocol`, `mcp server`, `agent skills`, `deepwiki`, `code graph`, `protocol generator`, `packet codec`;
- сгруппировать их по категориям;
- отфильтровать мертвые/игрушечные repos;
- вынести 20 необычных архитектурных идей;
- сохранить ссылки, stars, last update, languages, license, ключевые files;
- отдельно отметить "сомнительно, проверить руками".

### Стек A: полностью локальный, без внешней сильной модели

Компоненты:

- Ollama или LM Studio как OpenAI-compatible endpoint;
- SearXNG как metasearch;
- Local Deep Researcher или OpenDeepResearcher via SearXNG;
- Crawl4AI / Firecrawl self-hosted / TinySearch для извлечения страниц;
- локальная SQLite/Postgres база для найденных проектов;
- GitHub API token с read-only scopes или без token для public search, но с лимитами.

Что хорошо:

- приватность;
- можно оставить на ночь без платных API;
- хорошо для первичного сбора ссылок, дедупликации, кратких summary;
- можно подключить локальный clone McProtoNet и старого PacketGenerator.

Что плохо:

- локальная модель на GTX 1080 Ti часто будет слабее в judgment, чем внешняя;
- web search через SearXNG часто страдает от captcha, пустых результатов и SEO мусора;
- длинный context и много источников быстро ломают локальные 7B/14B модели;
- агент может уверенно завершить работу, даже если search backend фактически не работал.

### Стек B: локальный crawler + внешняя сильная модель для синтеза

Компоненты:

- GitHub API mining, SearXNG, Firecrawl, Rival Search MCP, TinySearch локально или self-hosted;
- локальное хранилище raw results;
- внешняя модель уровня GPT-5/Claude/Gemini для synthesis;
- structured output schema для результатов: repo, category, evidence, confidence, why interesting.

Что хорошо:

- crawler дешевый и controlled;
- сильная модель лучше видит архитектурные аналогии;
- можно заставить модель работать только по извлеченным evidence;
- проще получить хороший русский итоговый документ.

Что плохо:

- приватные материалы надо фильтровать до отправки;
- costs;
- нужен контроль hallucination: citations, "unknown", no source no claim.

### Стек C: browser-agent как дневной исследователь

Компоненты:

- browser-use или BrowserOS;
- Firecrawl web-agent для clean extraction;
- GitTrend/RepoRank/OSSInsight как стартовые поверхности;
- GitHub UI/API;
- Obsidian/Markdown output.

Что хорошо:

- browser-agent может читать dynamic pages, GitHub UI, docs sites;
- BrowserOS умеет scheduled tasks и local memory;
- Firecrawl web-agent уже заточен под structured web research.

Что плохо:

- browser agents уязвимы к prompt injection на страницах;
- stateful browser может случайно не туда кликнуть или сохранить session;
- нужен sandbox: отдельный browser profile, no secrets, no authenticated destructive actions.

### Стек D: GitHub mining без "браузерной магии"

Компоненты:

- GitHub REST/GraphQL API;
- OSSInsight для event data и SQL-like analysis;
- GitTrend/RepoRank/GitRepoTrend как candidate feeds;
- локальный скрипт ранжирования;
- LLM только для summary.

Пример полей для mining:

```json
{
  "repo": "owner/name",
  "topics": ["minecraft", "protocol", "mcp"],
  "stars": 1234,
  "forks": 120,
  "pushed_at": "2026-06-07",
  "created_at": "2025-11-01",
  "language": "C#",
  "license": "MIT",
  "signals": {
    "recent_commits": 42,
    "issues_recent": 12,
    "mentions_mcp": true,
    "mentions_skill": false
  },
  "why_interesting": "Uses generated protocol schemas and golden packet fixtures",
  "manual_check": true
}
```

Что хорошо:

- намного меньше hallucination surface;
- легко повторять раз в неделю;
- можно строить собственный "McProtoNet radar".

Что плохо:

- не видит смысла проекта глубже README без clone/scrape;
- raw stars/trending шумят;
- GitHub search API имеет rate limits и неполную семантику.

### GTX 1080 Ti 11GB: что реально локально

GTX 1080 Ti имеет 11 GB VRAM и старую Pascal-архитектуру. Это не современная inference-карта, но для локальных LLM все еще полезна, особенно с GGUF quantization.

Реалистичные режимы:

- 3B-4B модели: быстрые классификаторы, extraction, simple rerank, короткие summaries, tool routing.
- 7B-8B Q4/Q5/Q8: хороший локальный помощник для summary, фильтрации, rewrite, простого code understanding.
- 13B-14B Q4: часто помещается в 11 GB с умеренным context, но KV cache и overhead могут заставить снижать context/offload.
- 30B+ модели: возможны только с сильным CPU/RAM offload, медленно и неудобно для дневного agent loop.
- Embeddings/reranking: часто выгоднее локально, чем гонять через внешнюю модель.

Где локальная модель полезна:

- dedup search results;
- классификация repos по категориям;
- извлечение links/facts из README;
- короткие summaries по одному источнику;
- перевод/нормализация названий;
- подготовка JSON cards;
- предварительная проверка "это вообще связано с McProtoNet?".

Где лучше внешняя сильная модель:

- архитектурный синтез по 30-100 источникам;
- сравнение нескольких несовместимых подходов;
- нахождение скрытых assumptions;
- формулирование рисков и вопросов;
- русский long-form документ;
- проверка сложной задачи "что из этого реально переносимо в McProtoNet?".

Практичная схема для GTX 1080 Ti:

```text
SearXNG/GitHub API/Firecrawl
  -> локальная модель 7B/14B: clean, classify, summarize, rank
  -> SQLite/Markdown cards
  -> сильная внешняя модель: synthesis, contradictions, questions
  -> human review
```

### Задачи, которые можно оставить на день

- "Каждые 30 минут искать новые GitHub repos по topics `mcp-server`, `agent-skills`, `minecraft-protocol`, `deepwiki`, сохранять top 20 по recency + forks."
- "Собрать проекты, где протокол описан как JSON/IR/schema и генерируется код."
- "Найти Minecraft protocol libraries, которые честно документируют coverage matrix."
- "Найти MCP servers, которые используют C# SDK и resources/templates."
- "Найти examples of `SKILL.md` with scripts and security policy."
- "Сравнить DeepWiki/RepoMind/OpenDeepWiki output для McProtoNet и похожих protocol repos."
- "Построить список papers 2025-2026 по calibration, abstention, self-checking agents."

## Проблема уверенности ИИ и недопонимания

Пользовательский пример: задача была "объединить файлы", а агент сделал индекс/ссылки на старые файлы. Это не повод ругать конкретного агента. Это хороший мини-пример системного риска: агент уверенно выбрал близкую, но неверную интерпретацию операции.

Для McProtoNet это особенно опасно, потому что проект мультиверсионный и насыщен тонкими различиями:

- "поддерживает версию" может значить listed/generated/roundtrip/integration-tested;
- packet ID может зависеть от state, direction, protocol range;
- "обновить protocol" может значить обновить C# classes, generated catalogs, docs, tests, fixtures, MCP resources;
- "добавить packet" может быть schema-first, C#-first или diff-first;
- "объединить research" значит переписать и синтезировать, а не собрать индекс.

### Почему агенты звучат увереннее, чем знают

Факторы:

- language model оптимизирована продолжать правдоподобный текст;
- обычные benchmarks часто награждают ответ, а не честное "не знаю";
- агентный loop создает давление действовать;
- tool success не равен task success;
- user может принять аккуратный markdown за понимание;
- automation bias: человек начинает меньше проверять, когда output выглядит уверенно и структурно.

Свежие исследования про confidence-aware abstention и calibration прямо формулируют проблему: LLM часто дает уверенные, но неверные ответы, а self-reported confidence не всегда надежна без calibration. Human-in-the-loop тоже не магия: человек может rubber-stamp output, если интерфейс подталкивает доверять автоматике.

### Какие failure modes важны для McProtoNet

- **Misread task.** Агент путает "rewrite as one document" и "make an index".
- **Scope drift.** Агент затрагивает code/build files, хотя просили только docs.
- **False completeness.** Агент пишет "все версии поддержаны", не указав coverage evidence.
- **Stale source.** Агент берет wiki/blog 2022 вместо current docs.
- **Oracle confusion.** Агент считает PrismarineJS, Minecraft Wiki или ViaVersion абсолютной истиной.
- **Generated artifact confusion.** Агент не понимает, что является source of truth: C# attributes, generated code, JSON manifest или docs.
- **Tool overtrust.** Агент вызвал search tool, но search вернул мусор/пусто, а итог все равно выглядит уверенно.
- **Context inheritance.** На длинной задаче ранняя неверная гипотеза превращается в "факт" для следующих шагов.

### Как проектировать процесс, где агент обязан сомневаться

Не как "будь осторожен" в prompt, а как structure:

- task contract в начале: агент повторяет действие своими словами и выделяет destructive/write scope;
- ambiguity trigger: если запрос может значить разные операции, агент задает вопрос или делает минимальный безопасный вариант с явным assumption;
- evidence fields: каждый важный факт имеет source/evidence/confidence;
- abstention allowed: "не найдено", "не проверено", "не уверен" должно быть допустимым output;
- verification loop: после draft агент проверяет документ против checklist;
- structured outputs: intermediate JSON с `missingFacts`, `assumptions`, `needsHumanCheck`;
- evals: golden tasks, где правильное поведение - спросить уточнение или отказаться от догадки;
- human-in-the-loop не как rubber stamp, а как review конкретных uncertainties.

### Пример process rule для McProtoNet agents

```markdown
When a task asks to merge, consolidate, rewrite, replace, or supersede documents:
- Do not create an index unless the user explicitly asks for an index.
- Read the source documents as raw material.
- Produce a new self-contained document.
- Preserve source facts and links, but rewrite structure and prose.
- If unsure whether "merge" means "index" or "synthesize", ask before editing.
```

Такое правило лучше живет в `AGENTS.md` или docs skill, потому что это не project-specific packet fact, а workflow pitfall.

## Как объяснять тонкости McProtoNet ИИ

### 1. Canonical docs

Нужны короткие, стабильные документы, которые агент может прочитать быстро:

- `AGENTS.md`: repository map, commands, sensitive areas, no-commit/no-push, "do not infer protocol facts from memory".
- `docs/agent/overview.md`: что такое McProtoNet, какие проекты в `src/`, где tests/docs/benchmarks.
- `docs/agent/protocol-model.md`: state, direction, protocol id, Minecraft version, packet id range, `PacketInfo`, `PacketId`, `ProtocolSupport`.
- `docs/agent/serialization-contract.md`: VarInt, framing, compression, encryption boundary, no unread bytes, NBT/Slot/registry gotchas.
- `docs/agent/testing.md`: какие tests запускать для packet, NBT, source generator, docs.
- `docs/agent/known-pitfalls.md`: список реальных ошибок и edge cases.

### 2. Skills как процедуры

Возможный набор:

- `mcprotonet-packet-authoring`: добавить/изменить packet.
- `mcprotonet-protocol-update`: обновить Minecraft protocol version.
- `mcprotonet-serialization-performance`: менять hot path readers/writers.
- `mcprotonet-nbt`: NBT read/write/canonical equality.
- `mcprotonet-source-generator-review`: Roslyn/source generator changes.
- `mcprotonet-writerside-docs`: Writerside topics, code samples, diagrams.
- `mcprotonet-research-synthesis`: переписывать research как цельные documents, не indexes.

Форма skill:

```text
.agents/skills/mcprotonet-packet-authoring/
  SKILL.md
  references/
    packet-layout.md
    version-ranges.md
    test-matrix.md
  scripts/
    check-packet-index.ps1
```

В `SKILL.md` лучше держать routing и steps, а не всю энциклопедию протокола. Детали должны быть в references, generated catalogs или MCP resources.

### 3. MCP resources

McProtoNet MCP полезнее как read-only protocol workbench:

- `mcproto://solution/overview`;
- `mcproto://catalog/packets`;
- `mcproto://catalog/packets/{state}/{direction}`;
- `mcproto://packet/{state}/{direction}/{name}`;
- `mcproto://packet-id/{state}/{direction}/{id}?protocol={protocol}`;
- `mcproto://type/{name}`;
- `mcproto://versions`;
- `mcproto://docs/{topic}`;
- `mcproto://tests/index`;
- `mcproto://benchmarks/index`;
- `mcproto://pitfalls`;
- `mcproto://report/serialization/{id}`.

Resources должны быть маленькими и адресуемыми. Агент не должен читать 258 packet files, чтобы ответить про один packet.

### 4. MCP tools

Сначала pure local/read-only:

- `search_packets(query, version?, state?, direction?)`;
- `inspect_packet(name|id, version, state, direction)`;
- `compare_versions(name, state, direction, fromProtocol, toProtocol)`;
- `discover_schema(kind, name, protocol)`;
- `validate_serialization(packet, protocol, payload, roundtrip?)`;
- `search_docs(query)`;
- `suggest_tests_for_file(path)`;
- `get_public_api_summary(project)`.

Позже, только behind approval/dry-run:

- `generate_packet_skeleton(specRef, targetVersion)`;
- `update_protocol_map(sourceRef)`;
- `sync_docs_code_sample(sampleName)`;
- `run_targeted_tests(testFilter)`;
- `run_benchmark_short(benchmarkFilter)`.

### 5. JSON Schema и generated catalogs

Минимальная packet card:

```json
{
  "name": "UseItem",
  "class": "McProtoNet.Protocol.Packets.Play.Serverbound.UseItemPacket",
  "state": "play",
  "direction": "serverbound",
  "protocol": 772,
  "packetId": "0x40",
  "supportedProtocols": [{ "from": 735, "to": 772 }],
  "fields": [
    { "name": "Hand", "type": "VarInt" },
    { "name": "Sequence", "type": "VarInt" }
  ],
  "source": "src/McProtoNet.Protocol/Packets/Play/Serverbound/UseItemPacket.cs",
  "tests": [],
  "evidence": ["attributes", "generated-catalog"]
}
```

Слои:

- `PacketMetadata`: факты из attributes/source generator/catalog.
- `PacketPayloadSchema`: JSON Schema для payload.
- `SerializationTrace`: field -> primitive/type -> condition/version branch -> bytes.
- `Evidence`: source refs, tests, external oracle refs.

### 6. Golden tests и executable specs

Для ИИ важны не только unit tests, а tests как описание проекта:

- packet id uniqueness по `(version, state, direction, id)`;
- roundtrip serialize/deserialize;
- no unread bytes after deserialize;
- fixtures `.json` + `.bin`;
- negative fixtures: truncated payload, malformed VarInt, oversized NBT;
- differential fixtures: Prismarine/MCProtocolLib bytes;
- real server traces: status/login/configuration/play smoke;
- docs examples compile/run;
- evals для agent tasks.

### 7. Protocol traces

Trace format:

```json
{
  "timestamp": "2026-06-08T10:00:00Z",
  "protocol": 772,
  "state": "login",
  "direction": "serverbound",
  "packetId": "0x00",
  "packetName": "LoginStart",
  "compressed": false,
  "rawHex": "0007...",
  "decoded": {},
  "source": "headlessmc-smoke-1.21.8",
  "result": "accepted"
}
```

Trace объясняет агенту протокол лучше, чем prose: видно, что реально идет по сети, в каком state, в какой версии, после какого handshake.

### 8. Diagrams

Mermaid candidates:

- protocol state machine;
- handshake/status/login/configuration/play sequence;
- compression/encryption boundary;
- source generator pipeline;
- packet catalog generation;
- test pyramid: unit, fake harness, Docker server, HeadlessMC, differential oracle;
- MCP architecture: resources/tools/prompts.

Excalidraw candidates:

- radical architecture alternatives;
- old PacketGenerator decomposition;
- "source of truth" debate;
- multi-version graph vs per-version codec.

## Радикальные развилки

### Развилка 1: C# attributes как source of truth или schema-first IR

Вариант C#-first:

- packet classes и attributes являются правдой;
- source generator строит helpers;
- docs/MCP catalogs извлекаются из compiled/reflection/Roslyn view.

Вопросы:

- хватает ли reflection для version-specific fields?
- как фиксировать provenance от Minecraft Wiki/minecraft-data?
- не станет ли C# API слишком тяжелым для всех версий?

Вариант schema-first:

- отдельный protocol IR хранит packets/types/version ranges;
- C#, docs, tests, MCP resources генерируются из IR;
- C# становится output, а не primary schema.

Вопросы:

- как не потерять ergonomic .NET API?
- кто и как ревьюит IR changes?
- как мигрировать существующие packet classes?

Гибрид:

- C# остается runtime API;
- generated manifest становится evidence/report layer;
- IR появляется сначала как derived artifact, затем может стать source для новых versions.

### Развилка 2: MCP как локальная лаборатория или публичный протокольный сервис

Локальная лаборатория:

- stdio;
- read-only;
- работает с текущим checkout;
- без auth/secrets;
- tools для inspection/validation.

Публичный сервис:

- Streamable HTTP;
- auth/OAuth;
- public catalog;
- versioned API;
- registry listing;
- rate limits/audit.

Вопрос: McProtoNet MCP нужен только автору/контрибьюторам или может стать public "Minecraft protocol catalog for agents"?

### Развилка 3: "Все версии" как codec matrix или translation graph

Codec matrix:

- каждый packet описан для каждой версии;
- tests проверяют parse/write;
- проще для library users.

Translation graph:

- normalized model + transforms между версиями;
- ближе к ViaVersion;
- полезно для proxies и compatibility layers.

Вопрос: McProtoNet хочет быть codec library, translator, workbench или всеми слоями, но отдельно?

### Развилка 4: Полный Play state или thin vertical slices

Полный Play:

- амбициозная полнота;
- большая surface;
- сложно тестировать все семантически.

Thin slices:

- status ping;
- offline login;
- configuration transition;
- minimal play join;
- keepalive;
- disconnect/resource pack/chat.

Вопрос: какая единица "работает" важнее: packet coverage или scenario coverage?

### Развилка 5: AI writes code или AI prepares evidence

AI writes code:

- быстрее;
- риск ложных packet layouts и unrelated changes;
- нужен strong verification.

AI prepares evidence:

- собирает specs, diffs, packet cards, tests, traces;
- человек решает архитектуру и код;
- медленнее, но меньше риск "уверенно не понял".

Гипотеза: для McProtoNet долгое время самым ценным AI будет не author, а research/compiler/evidence assistant.

### Развилка 6: Docs site как human docs или docs-as-data

Human docs:

- Writerside topics;
- examples;
- guides.

Docs-as-data:

- generated JSON packet catalogs;
- `llms.txt`;
- MCP resources;
- API metadata;
- traces.

Вопрос: какой слой является продуктом? NuGet library, docs site, protocol dataset, MCP workbench или skill-pack?

## Что стоит проверить руками

1. Сравнить 5 packets в McProtoNet, `minecraft-data`, Minecraft Wiki и MCProtocolLib: ID, fields, state, direction, protocol range.
2. Выбрать один tricky packet с version-specific fields и попробовать описать его JSON Schema + SerializationTrace.
3. Прогнать DeepWiki/RepoMind/OpenDeepWiki-like tool по McProtoNet и записать, что оно поняло неправильно.
4. Поднять Local Deep Researcher + SearXNG и проверить, насколько он реально находит проекты, а не галлюцинирует при пустом search.
5. Запустить маленький GitHub API mining script по topics `minecraft-protocol`, `mcp-server`, `agent-skills`, `codegen`, `deepwiki`.
6. Попробовать LM Studio/Ollama на GTX 1080 Ti с 7B и 14B Q4: summary README, classify repos, extract JSON cards.
7. Проверить browser-use или BrowserOS в sandbox profile без секретов: GitHub browsing, docs extraction, no authenticated actions.
8. Сделать один `SKILL.md` для `mcprotonet-research-synthesis` и дать агенту повторить эту задачу на маленьком наборе файлов.
9. Сформулировать eval: "объедини 3 research files в self-contained doc" и считать ошибкой индекс ссылок.
10. Сделать минимальный `mcproto://catalog/packets` mock JSON и посмотреть, насколько агент лучше отвечает про packets.
11. Поднять `itzg/minecraft-server` через Testcontainers и получить первый status ping trace.
12. Запустить HeadlessMC против fake status server и сохранить raw trace.
13. Сравнить PacketEvents wrappers с McProtoNet public packet API: где version unsupported выражается явно, а где нет.
14. Посмотреть ViaProxy trace для старого client -> latest server и выписать, какие трансформации можно представить как graph.
15. Решить, какие claims в README/docs должны иметь support level: listed/generated/roundtrip/integration/real-client.

## Источники

### Локальные research-файлы, использованные как сырье

- `docs/ai-integration-research.md`
- `docs/research/combined-ai-protocol-technology-map.md`
- `docs/research/cross-agent-skills.md`
- `docs/research/documentation-knowledge-base.md`
- `docs/research/mcp-protocol-workbench.md`
- `docs/research/obsidian-excalidraw-workflow.md`
- `docs/research/protocol-testing-headlessmc.md`
- `docs/research/repo-workspace-architecture.md`
- `docs/research/technology-radar-food-for-thought.md`

### Agent instructions and skills

- AGENTS.md: <https://agents.md/>
- OpenAI Codex AGENTS.md guide: <https://developers.openai.com/codex/guides/agents-md>
- Agent Skills specification: <https://agentskills.io/specification>
- Anthropic skills repository: <https://github.com/anthropics/skills>
- Claude Code skills docs: <https://code.claude.com/docs/en/skills>
- Vercel skills CLI: <https://github.com/vercel-labs/skills>
- Vercel agent-skills: <https://github.com/vercel-labs/agent-skills>
- Vercel Agent Skills docs: <https://vercel.com/docs/agent-resources/skills>
- SkillsMD: <https://skillsmd.dev/>
- SkillsMD marketplace: <https://skillsmd.co/>
- skills.sh: <https://www.skills.sh/>
- Skillhound: <https://www.skillhound.ai/>
- Awesome Agent Skills: <https://github.com/skillcreatorai/Awesome-Agent-Skills>
- Skilldex: <https://arxiv.org/abs/2604.16911>
- SkillAttack: <https://arxiv.org/abs/2604.04989>
- SkillSieve: <https://arxiv.org/abs/2604.06550>
- Semantic supply-chain attacks on `SKILL.md`: <https://arxiv.org/abs/2605.11418>
- SWE-Skills-Bench: <https://arxiv.org/abs/2603.15401>

### MCP

- MCP architecture: <https://modelcontextprotocol.io/docs/learn/architecture>
- MCP specification: <https://modelcontextprotocol.io/specification/2025-11-25/basic/index>
- MCP resources: <https://modelcontextprotocol.io/specification/2025-11-25/server/resources>
- MCP tools: <https://modelcontextprotocol.io/specification/2025-11-25/server/tools>
- MCP prompts: <https://modelcontextprotocol.io/specification/2025-11-25/server/prompts>
- MCP transports: <https://modelcontextprotocol.io/specification/2025-11-25/basic/transports>
- MCP authorization: <https://modelcontextprotocol.io/specification/2025-11-25/basic/authorization>
- MCP security best practices: <https://modelcontextprotocol.io/docs/tutorials/security/security_best_practices>
- Official MCP Registry: <https://modelcontextprotocol.io/registry/about>
- MCP C# SDK: <https://github.com/modelcontextprotocol/csharp-sdk>
- MCP C# SDK docs: <https://csharp.sdk.modelcontextprotocol.io/>
- Docker MCP Gateway: <https://docs.docker.com/reference/cli/docker/mcp/gateway/gateway_run/>
- Glama: <https://glama.ai/>
- Smithery: <https://smithery.mintlify.dev/docs/build>
- OWASP MCP Tool Poisoning: <https://owasp.org/www-community/attacks/MCP_Tool_Poisoning>

### Docs, context, code graph

- `llms.txt`: <https://llmstxt.org/>
- Context7 docs: <https://context7.com/docs>
- DeepWiki: <https://deepwiki.org/>
- DeepWiki topic/open alternatives: <https://github.com/topics/deepwiki>
- RepoMind: <https://repomind.in/about>
- Sourcegraph OpenCtx article: <https://sourcegraph.com/blog/anatomy-of-a-coding-assistant>
- DocFX .NET API docs: <https://dotnet.github.io/docfx/docs/dotnet-api-docs.html>
- DocFX metadata CLI: <https://dotnet.github.io/docfx/reference/docfx-cli-reference/docfx-metadata.html>
- Writerside code docs: <https://www.jetbrains.com/help/writerside/code.html>
- Writerside Mermaid diagrams: <https://www.jetbrains.com/help/writerside/mermaid-diagrams.html>
- Docusaurus Markdown: <https://docusaurus.io/docs/markdown-features>
- Starlight Markdown authoring: <https://starlight.astro.build/guides/authoring-content/>
- Astro Content Collections: <https://docs.astro.build/en/guides/content-collections/>

### Research bots, search, browser agents, GitHub mining

- Local Deep Researcher: <https://github.com/langchain-ai/local-deep-researcher>
- Open Deep Research: <https://github.com/langchain-ai/open_deep_research>
- OpenDeepResearcher via SearXNG: <https://github.com/benhaotang/OpenDeepResearcher-via-searxng>
- SearXNG docs: <https://docs.searxng.org/user/about.html>
- browser-use: <https://github.com/browser-use/browser-use>
- BrowserOS: <https://www.browseros.com/>
- BrowserOS GitHub: <https://github.com/browseros-ai/BrowserOS>
- AgenticSeek: <https://github.com/andrewstack-maker/agenticSeek>
- Firecrawl web-agent: <https://github.com/firecrawl/web-agent>
- Firecrawl: <https://github.com/firecrawl>
- Rival Search MCP: <https://rivalsearchmcp.com/>
- TinySearch: <https://github.com/MarcellM01/TinySearch>
- GitTrend: <https://gittrend.io/>
- OSSInsight: <https://ossinsight.io/>
- OSSInsight Data Explorer: <https://ossinsight.io/explore/>
- OSSInsight public API: <https://ossinsight.io/docs/api/>
- RepoRank: <https://reporank.co/>
- GitRepoTrend: <https://gitrepotrend.com/>
- GitHub REST API: <https://docs.github.com/en/rest>

### Local models

- Ollama: <https://ollama.com/>
- LM Studio system requirements: <https://lmstudio.ai/docs/app/system-requirements/>
- llama.cpp: <https://github.com/ggml-org/llama.cpp>
- llama.cpp quantization evaluation: <https://arxiv.org/abs/2601.14277>

### Calibration, uncertainty, hallucinations, automation bias

- I-CALM confidence-aware abstention: <https://arxiv.org/abs/2604.03904>
- Dunning-Kruger effect in LLM calibration: <https://arxiv.org/abs/2603.09985>
- Calibrated trust and LLM hallucinations: <https://arxiv.org/abs/2512.09088>
- Automation bias review: <https://link.springer.com/article/10.1007/s00146-025-02422-7>
- CSET AI Safety and Automation Bias: <https://cset.georgetown.edu/publication/ai-safety-and-automation-bias/>
- Human-in-the-loop AI systematic review: <https://pmc.ncbi.nlm.nih.gov/articles/PMC13114286/>
- To Believe or Not to Believe Your LLM: <https://proceedings.nips.cc/paper_files/paper/2024/file/6aebba00fff5b6de7b488e496f80edd7-Paper-Conference.pdf>
- Knowing but Not Showing: LLMs Recognize Ambiguity but Rarely Ask Clarifying Questions: <https://www.researchgate.net/publication/405263657_Knowing_but_Not_Showing_LLMs_Recognize_Ambiguity_but_Rarely_Ask_Clarifying_Questions>

### Agentic codegen and evals

- OpenAI Structured Outputs: <https://platform.openai.com/docs/guides/structured-outputs>
- OpenAI Structured Outputs announcement: <https://openai.com/index/introducing-structured-outputs-in-the-api/>
- OpenAI Evals API: <https://platform.openai.com/docs/api-reference/evals>
- OpenAI Evals GitHub: <https://github.com/openai/evals>
- SWE-bench: <https://www.swebench.com/>
- FeatureBench: <https://featurebench.ai/>

### Minecraft/protocol

- Minecraft Wiki protocol documentation: <https://minecraft.wiki/w/Minecraft_Wiki:Protocol_documentation>
- PrismarineJS `minecraft-data`: <https://github.com/PrismarineJS/minecraft-data>
- PrismarineJS protocol pages: <https://prismarinejs.github.io/minecraft-data/protocol/>
- PrismarineJS `node-minecraft-protocol`: <https://github.com/PrismarineJS/node-minecraft-protocol>
- PrismarineJS mineflayer: <https://github.com/PrismarineJS/mineflayer>
- flying-squid: <https://github.com/PrismarineJS/flying-squid>
- MCProtocolLib: <https://github.com/GeyserMC/MCProtocolLib>
- HeadlessMC: <https://github.com/headlesshq/headlessmc>
- HeadlessMC docs: <https://headlesshq.github.io/headlessmc/>
- Minecraft Java server download: <https://www.minecraft.net/en-us/download/server>
- itzg Docker Minecraft Server: <https://github.com/itzg/docker-minecraft-server>
- Testcontainers for .NET: <https://dotnet.testcontainers.org/>
- Testcontainers wait strategies: <https://dotnet.testcontainers.org/api/wait_strategies/>
- GitHub Actions service containers: <https://docs.github.com/en/actions/tutorials/use-containerized-services/use-docker-service-containers>
- ViaVersion: <https://viaversion.com/>
- ViaBackwards: <https://github.com/ViaVersion/ViaBackwards>
- ViaRewind: <https://github.com/ViaVersion/ViaRewind>
- ViaLegacy: <https://github.com/ViaVersion/ViaLegacy>
- ViaProxy: <https://github.com/ViaVersion/ViaProxy>
- PacketEvents docs: <https://docs.packetevents.com/>
- PacketEvents GitHub: <https://github.com/retrooper/packetevents>
- ProtocolLib: <https://protocollib.org/>
- Minestom: <https://github.com/Minestom/Minestom>
- PicoLimbo: <https://github.com/Quozul/PicoLimbo>
- pyCraft: <https://github.com/ammaraskar/pyCraft>
- go-mc: <https://github.com/Tnze/go-mc>
- Pumpkin: <https://github.com/Pumpkin-MC/Pumpkin>
- Valence: <https://docs.rs/valence/latest/valence/>
- Azalea: <https://github.com/azalea-rs/azalea>

### Visual knowledge workflows

- Obsidian data storage: <https://obsidian.md/help/data-storage>
- Obsidian links: <https://obsidian.md/help/links>
- Obsidian backlinks: <https://obsidian.md/help/plugins/backlinks>
- Obsidian file formats: <https://obsidian.md/help/file-formats>
- Mermaid syntax reference: <https://mermaid.js.org/intro/syntax-reference.html>
- Obsidian Excalidraw plugin: <https://github.com/zsviczian/obsidian-excalidraw-plugin>
- JSON Canvas spec: <https://jsoncanvas.org/spec/1.0/>
- Dataview docs: <https://blacksmithgu.github.io/obsidian-dataview/>
