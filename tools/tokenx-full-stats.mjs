import { estimateTokenCount } from "tokenx";
import { readdir, readFile, stat } from "node:fs/promises";
import { extname, join } from "node:path";

const PACKETS_DIR = "src/McProtoNet.Protocol/Packets";
const TYPES_DIR = "src/McProtoNet.Protocol/Types";
const EXTENSIONS_DIR = "src/McProtoNet.Protocol/Extensions";
const PERCENTILES = [50, 75, 90, 95, 99];

async function walk(dir) {
  const entries = await readdir(dir, { withFileTypes: true });
  const files = [];
  for (const entry of entries) {
    const fullPath = join(dir, entry.name);
    if (entry.isDirectory()) {
      files.push(...(await walk(fullPath)));
    } else if (entry.isFile() && extname(entry.name).toLowerCase() === ".cs") {
      files.push(fullPath);
    }
  }
  return files;
}

function percentile(sortedValues, p) {
  if (!sortedValues.length) return 0;
  const idx = (p / 100) * (sortedValues.length - 1);
  const lo = Math.floor(idx);
  const hi = Math.ceil(idx);
  if (lo === hi) return sortedValues[lo];
  const weight = idx - lo;
  return sortedValues[lo] * (1 - weight) + sortedValues[hi] * weight;
}

function computeStats(values) {
  if (!values.length) return { min: 0, max: 0, avg: 0, percentiles: {} };
  values.sort((a, b) => a - b);
  const total = values.reduce((sum, v) => sum + v, 0);
  const avg = total / values.length;
  const min = values[0];
  const max = values[values.length - 1];
  const percentiles = Object.fromEntries(
    PERCENTILES.map((p) => [p, percentile(values, p)])
  );
  return { min, max, avg, percentiles };
}

async function analyzeDir(dir, category) {
  const files = await walk(dir);
  const stats = [];
  for (const file of files) {
    const content = await readFile(file, "utf8");
    const tokens = estimateTokenCount(content);
    const fileInfo = await stat(file);
    stats.push({ file, tokens, bytes: fileInfo.size });
  }
  const tokenValues = stats.map((s) => s.tokens);
  const summary = computeStats(tokenValues);
  return { category, dir, fileCount: stats.length, tokens: summary, perFile: stats };
}

async function main() {
  const [packets, types, extensions] = await Promise.all([
    analyzeDir(PACKETS_DIR, "packets"),
    analyzeDir(TYPES_DIR, "types"),
    analyzeDir(EXTENSIONS_DIR, "extensions"),
  ]);

  const allFiles = [...packets.perFile, ...types.perFile, ...extensions.perFile];
  const allTokens = allFiles.map((f) => f.tokens);
  const allStats = computeStats(allTokens);

  const result = {
    summary: {
      totalFiles: allFiles.length,
      totalTokens: allTokens.reduce((sum, v) => sum + v, 0),
      tokens: allStats,
    },
    byCategory: {
      packets,
      types,
      extensions,
    },
  };

  console.log(JSON.stringify(result, null, 2));
}

main().catch((err) => {
  console.error(err);
  process.exit(1);
});
