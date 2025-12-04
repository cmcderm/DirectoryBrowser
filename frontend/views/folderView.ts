import type { PathInfo } from '../models/PathInfo.js';

// /app + / + folder => /app/folder
// /app + folder + anotherFolder => /app/folder/anotherFolder
// /app + folder/folder1 + lastFolder => /app/folder/folder1/lastFolder
const buildLink = (base: string, path: string, target: string): string => {
  if (path === '/') {
    return `${base}${path}${target}`;
  } else {
    return `${base}${path}/${target}`
  }
}

export const showFolder = (app: HTMLElement, params: Record<string, string>) => {
  const contentContainer = document.createElement('div');
  contentContainer.classList.add('content-container');
  app.append(contentContainer);

  const folderContainer = document.createElement('div');
  folderContainer.classList.add('folder-container');

  contentContainer.append(folderContainer);

  let path = params['path'];
  if (path === '' || path === undefined) {
    path = '/';
  }

  let titleHeader = document.createElement('h1');
  titleHeader.textContent = path;
  folderContainer.append(titleHeader);

  // Loading
  let loading = document.createElement('div');
  loading.textContent = 'Loading folder...';
  folderContainer.append(loading);

  console.log(path);

  fetch(`/api/path${path}`)
    .then(response => response.json())
    .then(data => {
      const pathInfo: PathInfo | undefined = data as PathInfo;

      if (!data) { return; }

      // Clear the loading text
      folderContainer.textContent = '';

      let titleHeader = document.createElement('h1');
      titleHeader.textContent = path;
      folderContainer.append(titleHeader);

      // Create each folder or file entry
      if (pathInfo && pathInfo.entries) {
        for (const entry of pathInfo.entries) {
          console.log(entry);

          const entryElement = document.createElement('div');
          entryElement.classList.add('folder-entry');
          folderContainer.appendChild(entryElement);

          const linkElement = document.createElement('a');
          linkElement.href = buildLink('/app', path, entry.name ?? '');
          linkElement.textContent = entry.name ?? '';
          entryElement.appendChild(linkElement);
        }
      }

      if (pathInfo.fileContents) {
        const fileContents = document.createElement('div');
        fileContents.classList.add('file-contents');
        fileContents.textContent = pathInfo.fileContents;
        folderContainer.append(fileContents);
      }
    })
    .catch(err => {
      console.error('Error fetching path info:', err);
    });
}
