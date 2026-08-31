import {themes as prismThemes} from 'prism-react-renderer';
import type {Config} from '@docusaurus/types';
import type * as Preset from '@docusaurus/preset-classic';

import {execSync} from 'node:child_process';

import remarkMcVersions from './src/remark/mc-versions';

/**
 * Версия для полосы сверху берётся из последнего git-тега, а не пишется
 * руками: номер превью считает MinVer по тем же тегам, и любая копия
 * в тексте однажды отстанет. Если тегов нет (мелкий клон, архив без .git),
 * полоса обходится без номера.
 */
function taggedVersion(): string | null {
  try {
    const tag = execSync('git describe --tags --abbrev=0 --match "v*"', {
      cwd: __dirname,
      encoding: 'utf8',
      stdio: ['ignore', 'pipe', 'ignore'],
    }).trim();
    return tag.replace(/^v/, '') || null;
  } catch {
    return null;
  }
}

const version = taggedVersion();

/**
 * Полоса сверху не переводится через `i18n`: Docusaurus не заводит для неё
 * слот, содержимое остаётся сырым HTML из конфига. Язык берём из окружения
 * сборки - конфиг читается заново на каждую локаль.
 */
const locale = process.env.DOCUSAURUS_CURRENT_LOCALE ?? 'en';

const announcement =
  locale === 'ru'
    ? [
        version
          ? `Это документация к <b>${version}</b>, версия ещё в работе.`
          : 'Это документация к 2.0, версия ещё в работе.',
        'Без <code>--prerelease</code> NuGet поставит стабильную 1.x,',
        'её описывает <a href="https://titlehhhh.github.io/McProtoNet/">старая документация</a>.',
      ].join(' ')
    : [
        version
          ? `This documents <b>${version}</b>, still in development.`
          : 'This documents 2.0, still in development.',
        'Without <code>--prerelease</code> NuGet installs the stable 1.x,',
        'described by the <a href="https://titlehhhh.github.io/McProtoNet/">old documentation</a>.',
      ].join(' ');

const config: Config = {
  title: 'McProtoNet',
  tagline: 'The Minecraft protocol in C#',
  favicon: 'img/favicon.svg',

  future: {
    v4: true,
    faster: true,
  },

  // GitHub Pages: project site у пользователя Titlehhhh. Новый сайт живёт
  // на /next/, в корне пока публикуется старая документация Writerside.
  url: 'https://titlehhhh.github.io',
  baseUrl: '/McProtoNet/next/',
  organizationName: 'Titlehhhh',
  projectName: 'McProtoNet',
  trailingSlash: false,

  // Сборка должна падать на битой ссылке, а не выпускать её в мир.
  onBrokenLinks: 'throw',
  onBrokenAnchors: 'throw',
  onDuplicateRoutes: 'throw',

  // Английский — основной, русский живёт на /ru/.
  i18n: {
    defaultLocale: 'en',
    locales: ['en', 'ru'],
    localeConfigs: {
      en: {label: 'English'},
      ru: {label: 'Русский'},
    },
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
      content: announcement,
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
      // Схемы рисуются палитрой сайта, а не серым по умолчанию.
      options: {
        fontFamily:
          'Inter, system-ui, -apple-system, "Segoe UI", Roboto, sans-serif',
        themeVariables: {
          background: '#0e1013',
          primaryColor: '#191d22',
          primaryBorderColor: '#626d7c',
          primaryTextColor: '#d9dde3',
          secondaryColor: '#13161a',
          tertiaryColor: '#13161a',
          lineColor: '#8b95a3',
          textColor: '#d9dde3',
          mainBkg: '#191d22',
          nodeBorder: '#626d7c',
          clusterBkg: '#13161a',
          clusterBorder: '#333b46',
          edgeLabelBackground: '#13161a',
        },
      },
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
          label: 'Docs',
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
        {
          href: 'https://discord.gg/PWfYWRDJme',
          label: 'Discord',
          position: 'right',
        },
        {
          type: 'localeDropdown',
          position: 'right',
        },
      ],
    },
    footer: {
      style: 'dark',
      links: [
        {
          title: 'Docs',
          items: [
            {label: 'About the project', to: '/docs/overview/about'},
            {label: 'Installation', to: '/docs/getting-started/installation'},
            {label: 'First bot', to: '/docs/getting-started/first-bot'},
            {label: 'Glossary', to: '/docs/reference/glossary'},
          ],
        },
        {
          title: 'Packages',
          items: [
            {
              label: 'NuGet',
              href: 'https://www.nuget.org/packages/McProtoNet',
            },
            {
              label: 'Nightly builds',
              href: 'https://f.feedz.io/mcprotonet/night/nuget/index.json',
            },
          ],
        },
        {
          title: 'Project',
          items: [
            {
              label: 'GitHub',
              href: 'https://github.com/Titlehhhh/McProtoNet',
            },
            {
              label: 'Issues',
              href: 'https://github.com/Titlehhhh/McProtoNet/issues',
            },
            {
              label: 'Discord',
              href: 'https://discord.gg/PWfYWRDJme',
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
