import { defineConfig } from 'vitepress'

const zhNav = [
  { text: '文档', link: '/guide/getting-started' },
  { text: '赞助', link: '/sponsors' },
  {
    text: '生态',
    items: [
      {
        text: 'Dddify Admin',
        link: 'https://github.com/esofar/dddify-admin',
      }
    ]
  }
]

const enNav = [
  { text: 'Docs', link: '/en/guide/getting-started' },
  { text: 'Sponsors', link: '/en/sponsors' },
  {
    text: 'Ecosystem',
    items: [
      {
        text: 'Dddify Admin',
        link: 'https://github.com/esofar/dddify-admin',
      }
    ]
  }
]

const zhSidebar = {
  '/': [
    {
      text: '指南',
      items: [
        { text: '快速开始', link: '/guide/getting-started' },
        { text: '框架配置', link: '/guide/configuration' },
        { text: '依赖注入', link: '/guide/dependency-injection' },
        { text: '领域建模', link: '/guide/domain-model' },
        { text: '应用编排', link: '/guide/application-flow' },
      ]
    },
    {
      text: '模块',
      items: [
        { text: '数据持久化', link: '/modules/entity-framework-core' },
        { text: '时间与时区', link: '/modules/timing' },
        { text: '当前用户', link: '/modules/current-user' },
        { text: '本地化', link: '/modules/localization' },
        { text: '结果包装', link: '/modules/api-result-wrapping' }
      ]
    }
  ]
}

const enSidebar = {
  '/en/': [
    {
      text: 'Guide',
      items: [
        { text: 'Getting Started', link: '/en/guide/getting-started' },
        { text: 'Configuration', link: '/en/guide/configuration' },
        { text: 'Dependency Injection', link: '/en/guide/dependency-injection' },
        { text: 'Domain Modeling', link: '/en/guide/domain-model' },
        { text: 'Application Flowing', link: '/en/guide/application-flow' },
      ]
    },
    {
      text: 'Modules',
      items: [
        { text: 'Data Persistence', link: '/en/modules/entity-framework-core' },
        { text: 'Time & Time Zone', link: '/en/modules/timing' },
        { text: 'Current User', link: '/en/modules/current-user' },
        { text: 'Localization', link: '/en/modules/localization' },
        { text: 'Result Wrapping', link: '/en/modules/api-result-wrapping' }
      ]
    }
  ]
}

export default defineConfig({
  title: 'Dddify',
  description: 'A lightweight DDD-based integration framework for modern ASP.NET Core applications.',
  srcDir: 'src',
  base: '/',
  cleanUrls: true,
  head: [
    ['link', { rel: 'icon', type: 'image/svg+xml', href: `favicon.svg` }],
    ['link', { rel: 'alternate icon', href: `favicon.ico` }]
  ],

  locales: {
    root: {
      label: '简体中文',
      lang: 'zh-CN',
      link: '/',
      description: '面向现代 ASP.NET Core 应用的轻量级 DDD 集成框架。',
      themeConfig: {
        nav: zhNav,
        sidebar: zhSidebar,
        outline: {
          level: [2, 2],
          label: '本页目录'
        },
        docFooter: {
          prev: '上一页',
          next: '下一页'
        }
      }
    },
    en: {
      label: 'English',
      lang: 'en-US',
      link: '/en/',
      description: 'A lightweight DDD-based integration framework for modern ASP.NET Core applications.',
      themeConfig: {
        nav: enNav,
        sidebar: enSidebar,
        outline: {
          level: [2, 2],
          label: 'On this page'
        },
        docFooter: {
          prev: 'Previous page',
          next: 'Next page'
        }
      }
    }
  },

  themeConfig: {
    logo: {
      light: '/hero-light.png',
      dark: '/hero-dark.png',
    },

    search: {
      provider: 'local'
    },

    socialLinks: [
      { icon: 'github', link: 'https://github.com/esofar/dddify' }
    ],

    footer: {
      message: 'Released under the MIT License.',
      copyright: 'Copyright (c) esofar'
    }
  },

  markdown: {
    lineNumbers: true
  }
})
