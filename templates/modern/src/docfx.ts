// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

import 'bootstrap'
import { DocfxOptions } from './options'
import { highlight } from './highlight'
import { renderMarkdown } from './markdown'
import { enableSearch } from './search'
import { renderToc } from './toc'
import { initTheme } from './theme'
import { renderBreadcrumb, renderInThisArticle, renderNavbar } from './nav'

import 'bootstrap-icons/font/bootstrap-icons.scss'
import './docfx.scss'

declare global {
  interface Window {
    docfx: DocfxOptions & {
      ready?: boolean,
      searchReady?: boolean,
      searchResultReady?: boolean,
    }
  }
}

export async function init(options: DocfxOptions) {
  window.docfx = Object.assign({}, options)

  initTheme()
  enableSearch()
  renderInThisArticle()

  await Promise.all([
    renderMarkdown(),
    renderNav(),
    highlight()
  ])

  window.docfx.ready = true

  async function renderNav() {
    const [navbar, toc] = await Promise.all([renderNavbar(), renderToc()])
    if (navbar === undefined) {
      throw new Error('navbar is undefined')
    }
    renderBreadcrumb([...navbar, ...toc])
  }
}
