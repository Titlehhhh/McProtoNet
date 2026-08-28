import {themes as prismThemes} from 'prism-react-renderer';
import type {Config} from '@docusaurus/types';
import type * as Preset from '@docusaurus/preset-classic';

import remarkMcVersions from './src/remark/mc-versions';

const config: Config = {
  title: 'McProtoNet',
  tagline: 'Протокол Minecraft на C#',
  favicon: 'img/favicon.svg',

  future: {
    v4: true,
    faster: true,
  },

  // GitHub Pages: project site у пользователя Titlehhhh.
  url: 'https://titlehhhh.github.io',
  baseUrl: '/McProtoNet/',
  organizationName: 'Titlehhhh',
  projectName: 'McProtoNet',
  trailingSlash: false,

  // Сборка должна падать на битой ссылке, а не выпускать её в мир.
  onBrokenLinks: 'throw',
  onBrokenAnchors: 'throw',
  onDuplicateRoutes: 'throw',

  // Русский — основной. Английский приезжает, когда русские тексты приняты.
  i18n: {
    defaultLocale: 'ru',
    locales: ['ru'],
  },

  markdown: {
    mermaid: true,
    hooks: {
      onBrokenMarkdownLinks: 'warn',
    },
  },

  themes: [
    '@docusaurus/theme-mermaid',
    [
      // Поиск целиком статический: индекс собирается при сборке и лежит
      // рядом с сайтом, свой сервер и Algolia не нужны.
      // Внимание: в `npm start` поиск не работает — весь клиентский код
      // обёрнут в проверку на production. Проверять через build + serve.
      '@easyops-cn/docusaurus-search-local',
      {
        language: ['ru', 'en'],
        hashed: 'filename',
        indexDocs: true,
        indexBlog: false,
        indexPages: true,
        docsRouteBasePath: '/docs',
        docsDir: 'docs',
        highlightSearchTermsOnTargetPage: true,
        explicitSearchResultPath: true,
        searchResultLimits: 8,
        searchBarShortcutKeymap: 'mod+k',
      },
    ],
  ],

  presets: [
    [
      'classic',
      {
        docs: {
          path: 'docs',
          routeBasePath: 'docs',
          sidebarPath: './sidebars.ts',
          // edit/, а не tree/ — иначе кнопка ведёт на просмотр, а не на правку.
          editUrl:
            'https://github.com/Titlehhhh/McProtoNet/edit/master/docs/site/',
          remarkPlugins: [remarkMcVersions],
          showLastUpdateTime: true,
          showLastUpdateAuthor: false,
        },
        blog: false,
        theme: {
          customCss: './src/css/custom.css',
        },
        // changefreq и priority Google давно игнорирует; полезен lastmod,
        // и он работает только вместе с showLastUpdateTime выше.
        sitemap: {lastmod: 'date', priority: null, changefreq: null},
      } satisfies Preset.Options,
    ],
  ],

  themeConfig: {
    image: 'img/social-card.png',
    announcementBar: {
      id: 'preview-2-0',
      content: 'Документация к 2.0.0-preview.4. API ещё может меняться.',
      isCloseable: true,
    },
    // Прототип был тёмным и только тёмным — здесь так же.
    colorMode: {
      defaultMode: 'dark',
      disableSwitch: true,
      respectPrefersColorScheme: false,
    },
    docs: {
      sidebar: {
        hideable: true,
        autoCollapseCategories: true,
      },
    },
    tableOfContents: {
      minHeadingLevel: 2,
      maxHeadingLevel: 4,
    },
    mermaid: {
      theme: {light: 'neutral', dark: 'dark'},
    },
    navbar: {
      title: 'McProtoNet',
      logo: {
        alt: 'McProtoNet',
        src: 'img/logo.svg',
      },
      items: [
        {
          type: 'docSidebar',
          sidebarId: 'docs',
          position: 'left',
          label: 'Документация',
        },
        {
          href: 'https://www.nuget.org/packages/McProtoNet',
          label: 'NuGet',
          position: 'right',
        },
        {
          href: 'https://github.com/Titlehhhh/McProtoNet',
          label: 'GitHub',
          position: 'right',
        },
      ],
    },
    footer: {
      style: 'dark',
      links: [
        {
          title: 'Документация',
          items: [
            {label: 'Об проекте', to: '/docs/overview/about'},
            {label: 'Первый бот', to: '/docs/getting-started/minimal-bot'},
            {label: 'Что библиотека не делает', to: '/docs/overview/non-goals'},
            {label: 'Словарь', to: '/docs/reference/glossary'},
          ],
        },
        {
          title: 'Пакеты',
          items: [
            {
              label: 'NuGet',
              href: 'https://www.nuget.org/packages/McProtoNet',
            },
            {
              label: 'Ночные сборки',
              href: 'https://f.feedz.io/mcprotonet/night/nuget/index.json',
            },
          ],
        },
        {
          title: 'Проект',
          items: [
            {
              label: 'GitHub',
              href: 'https://github.com/Titlehhhh/McProtoNet',
            },
            {
              label: 'Issues',
              href: 'https://github.com/Titlehhhh/McProtoNet/issues',
            },
          ],
        },
      ],
      copyright: `McProtoNet, ${new Date().getFullYear()}.`,
    },
    prism: {
      theme: prismThemes.github,
      darkTheme: prismThemes.dracula,
      // Имя должно совпадать с компонентом Prism: для XML это markup,
      // и он включён по умолчанию.
      additionalLanguages: [
        'csharp',
        'fsharp',
        'bash',
        'json',
        'powershell',
        'diff',
        'yaml',
      ],
    },
  } satisfies Preset.ThemeConfig,
};

export default config;
