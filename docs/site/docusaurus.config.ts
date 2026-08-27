import {themes as prismThemes} from 'prism-react-renderer';
import type {Config} from '@docusaurus/types';
import type * as Preset from '@docusaurus/preset-classic';

import remarkMcVersions from './src/remark/mc-versions';

const config: Config = {
  title: 'McProtoNet',
  tagline: 'Протокол Minecraft на C#',
  favicon: 'img/favicon.ico',

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

  onBrokenLinks: 'throw',

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
  themes: ['@docusaurus/theme-mermaid'],

  presets: [
    [
      'classic',
      {
        docs: {
          path: 'docs',
          routeBasePath: 'docs',
          sidebarPath: './sidebars.ts',
          editUrl:
            'https://github.com/Titlehhhh/McProtoNet/tree/master/docs/site/',
          remarkPlugins: [remarkMcVersions],
        },
        blog: false,
        theme: {
          customCss: './src/css/custom.css',
        },
      } satisfies Preset.Options,
    ],
  ],

  themeConfig: {
    image: 'img/docusaurus-social-card.jpg',
    colorMode: {
      respectPrefersColorScheme: true,
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
            {label: 'Что библиотека не делает', to: '/docs/overview/non-goals'},
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
              label: 'NuGet',
              href: 'https://www.nuget.org/packages/McProtoNet',
            },
          ],
        },
      ],
      copyright: `McProtoNet, ${new Date().getFullYear()}.`,
    },
    prism: {
      theme: prismThemes.github,
      darkTheme: prismThemes.dracula,
      additionalLanguages: ['csharp', 'fsharp', 'bash', 'json'],
    },
  } satisfies Preset.ThemeConfig,
};

export default config;
