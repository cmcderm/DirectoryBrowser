import type { DirectoryEntry } from "./DirectoryEntry.js";

export interface PathInfo {
  path?: string;
  isDirectory?: boolean;
  entries?: Array<DirectoryEntry>;
  fileContents?: string;
}

