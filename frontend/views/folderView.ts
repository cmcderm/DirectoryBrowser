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

const uploadFile = (path: string, file?: File) => {
  if (file === undefined) { return; }

  const form = new FormData();
  form.append("file", file);

  fetch(`/api/path/upload${path}`,
    {
      method: 'POST',
      body: form
    }
  ).then(r => r.json())
   .then(_ => {
      console.log("Upload complete");
      window.location.reload();
    });
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

      let dirInfoContainer = document.createElement('div');
      dirInfoContainer.textContent =
        `${pathInfo.folderCount} folders, ${pathInfo.fileCount} files, total size ${pathInfo.size}B`;
      folderContainer.append(dirInfoContainer);

      if (pathInfo.isDirectory) {
        let uploadText = document.createElement('span');
        uploadText.textContent = "Upload File: "
        folderContainer.appendChild(uploadText);

        let input = document.createElement('input');
        input.type = "file";
        input.onchange = () => {
          if (input.files && input.files.length > 0) {
            uploadFile(path, input.files[0]);
          }
        }
        folderContainer.appendChild(input);
      }

      // Create each folder or file entry
      if (pathInfo && pathInfo.entries) {
        for (const entry of pathInfo.entries) {
          const entryElement = document.createElement('div');
          entryElement.classList.add('folder-entry');
          folderContainer.appendChild(entryElement);

          const linkElement = document.createElement('a');
          linkElement.href = buildLink('/app', path, entry.name ?? '');
          linkElement.textContent = entry.name ?? '';
          entryElement.appendChild(linkElement);

          const infoContainer = document.createElement('div');
          infoContainer.textContent =
            `${entry.folderCount} folders, ${entry.fileCount} files, total size ${entry.size}B`;
          linkElement.appendChild(infoContainer);

          if (!entry.isDirectory) {
            console.log(entry);
            const downloadLinkElement = document.createElement('button');
            downloadLinkElement.textContent = 'Download';
            downloadLinkElement.onclick = () => {
              window.location.href = `/api/path/download${path}/${entry.name}`
            }
            entryElement.appendChild(downloadLinkElement);
          }
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

