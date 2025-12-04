import { init } from './router.js';

const root = document.getElementById('app');

if (!root) {
  throw new Error("Root element with id 'app' not found");
}

init(root);

