import { defineConfig, type DefaultTheme } from 'vitepress'
import pkg from '../package.json' assert { type: 'json' }

// https://vitepress.dev/reference/site-config
export default defineConfig({
  lang: 'zh',
  title: "Dddify",
  description: "A VitePress Site",
  srcDir: 'src',
  cleanUrls: true,
  locales: {
    root: { label: '简体中文', lang: 'zh' },
  },
  head: [
    ['link', { rel: 'icon', href: '/favicon.ico' }]
  ],
  themeConfig: {
    nav: nav(),
    sidebar: {
      '/guide/': { base: '/guide', items: sidebarGuide() },
    },
    logo: {
      light: '/site-title-light.png',
      dark: '/site-title-dark.png',
    },
    siteTitle: false,
    editLink: {
      pattern: 'https://github.com/esofar/dddify/edit/main/docs/:path',
      text: '在 GitHub 上编辑此页面'
    },
    footer: {
      message: '基于 MIT 许可发布',
      copyright: `Copyright © 2023-${new Date().getFullYear()} Dddify`
    },
    docFooter: {
      prev: '上一页',
      next: '下一页'
    },
    outline: {
      label: '页面导航'
    },
    lastUpdated: {
      text: '最后更新于',
      formatOptions: {
        dateStyle: 'short',
        timeStyle: 'medium'
      }
    },
    socialLinks: [
      { icon: 'github', link: 'https://github.com/esofar/dddify' }
    ],
    search: {
      provider: 'local',
      options: localSearchOptions()
    },
    langMenuLabel: '多语言',
    returnToTopLabel: '回到顶部',
    sidebarMenuLabel: '菜单',
    darkModeSwitchLabel: '主题',
    lightModeSwitchTitle: '切换到浅色模式',
    darkModeSwitchTitle: '切换到深色模式'
  }
})

function nav(): DefaultTheme.NavItem[] {
  return [
    {
      text: '指南',
      link: '/guide/introduction'
    },
    {
      text: '赞助',
      link: '/sponsor'
    },
    {
      text: '生态',
      items: [
        {
          text: 'Dddify Admin',
          link: 'https://github.com/esofar/dddify-admin',
        }
      ]
    },
    {
      text: pkg.version,
      items: [
        {
          text: '更新日志',
          link: 'https://github.com/esofar/dddify/blob/main/CHANGELOG.md',
        },
        {
          text: '参与贡献',
          link: 'https://github.com/esofar/dddify/blob/main/.github/contributing.md',
        }
      ]
    }
  ]
}

function sidebarGuide(): DefaultTheme.SidebarItem[] {
  return [
    {
      text: '开始',
      collapsed: false,
      items: [
        { text: '项目介绍', link: '/introduction' },
        { text: '分层架构', link: '/project-structure' },
        { text: '快速上手', link: '/getting-started' },
      ]
    },
    {
      text: '领域层',
      collapsed: false,
      items: [
        { text: '实体', link: '/entities' },
        { text: '值对象', link: '/value-objects' },
        { text: '聚合根', link: '/aggregate-roots' },
        { text: '领域服务', link: '/domain-services' },
        { text: '领域事件', link: '/domain-events' },
        { text: '仓储接口', link: '/repositories' },
      ]
    },
    {
      text: '应用层',
      collapsed: false,
      items: [
        { text: '命令与查询', link: '/cqrs-pattern' },
        { text: '管道行为', link: '/cqrs-pattern' },
        { text: '参数校验', link: '/validation' },
        { text: '对象映射', link: '/object-mapping' },
        { text: '领域事件处理器', link: '/domain-event-handler' },
      ]
    },
    {
      text: '基础设施层',
      collapsed: false,
      items: [
        { text: '数据持久化', link: '/persistence' },
        { text: '工作单元', link: '/unitofwork' },
        { text: '当前用户', link: '/current-user' },
        { text: 'GUID 生成', link: '/guid-generation' },
        { text: '并发检查', link: '/concurrency-check' },
      ]
    },
    {
      text: '用户界面层',
      collapsed: false,
      items: [
        { text: 'JSON 本地化', link: '/i18n' },
        { text: 'API 结果包装', link: '/api-result-warp' },
      ]
    },
    {
      text: '其他主题',
      collapsed: false,
      items: [
        { text: '依赖注入', link: '/dependency-injection' },
        { text: '异常处理', link: '/exception-handling' },
        { text: '时间服务', link: '/clock' },
        { text: '实用扩展', link: '/clock' },
      ]
    }
  ]
}

function localSearchOptions(): DefaultTheme.LocalSearchOptions {
  return {
    locales: {
      root: {
        translations: {
          button: {
            buttonText: '搜索文档',
            buttonAriaLabel: '搜索文档'
          },
          modal: {
            noResultsText: '无法找到相关结果',
            resetButtonTitle: '清除查询条件',
            footer: {
              selectText: '选择',
              navigateText: '切换',
              closeText: '关闭',
            }
          }
        }
      }
    }
  };
}