import type { DirectoryEntry } from "./DirectoryEntry.js";

export interface PathInfo {
  path?: string;
  isDirectory?: boolean;
  entries?: Array<DirectoryEntry>;
  folderCount?: number;
  fileCount?: number;
  size?: number;
  fileContents?: string;
}

