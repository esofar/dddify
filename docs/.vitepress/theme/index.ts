import DefaultTheme from 'vitepress/theme'
import MyLayout from './layouts/MyLayout.vue'

import './styles/custom.css'

export default {
    extends: DefaultTheme,
    Layout: MyLayout
  }
