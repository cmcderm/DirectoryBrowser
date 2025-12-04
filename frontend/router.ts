import { showFolder } from './views/folderView.js';

type ViewRenderer = (app: HTMLElement, params: Record<string, string>) => void;

const routeBase = '/app'

// Render
const renderRoute = (path: string, app: HTMLElement) => {
  const renderer: ViewRenderer = showFolder;

  const params = {
    path: path.replace('/app', ''),
  }

  app.innerHTML = '';
  renderer(app, params);
}

const navigate = (path: string, app: HTMLElement) => {
  history.pushState({}, '', path);
  renderRoute(path, app);
}

const init = (app: HTMLElement) => {
  renderRoute(window.location.pathname, app);

  window.onpopstate = () => {
    renderRoute(window.location.pathname, app);
  }

  (window as any).navigate = (path: string) => {
    navigate(path, app);
  }
}

export { init, navigate };
