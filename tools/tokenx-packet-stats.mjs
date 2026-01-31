import { estimateTokenCount } from "tokenx";
import { readdir, readFile, stat } from "node:fs/promises";
import { extname, join } from "node:path";

const PACKETS_DIR = "src/McProtoNet.Protocol/Packets";
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

async function main() {
  const packetFiles = await walk(PACKETS_DIR);
  const stats = [];

  for (const file of packetFiles) {
    const content = await readFile(file, "utf8");
    const tokens = estimateTokenCount(content);
    const fileInfo = await stat(file);
    stats.push({ file, tokens, bytes: fileInfo.size });
  }

  stats.sort((a, b) => a.tokens - b.tokens);
  const tokenValues = stats.map((s) => s.tokens);

  const total = tokenValues.reduce((sum, v) => sum + v, 0);
  const avg = tokenValues.length ? total / tokenValues.length : 0;
  const min = tokenValues[0] ?? 0;
  const max = tokenValues[tokenValues.length - 1] ?? 0;

  const percentiles = Object.fromEntries(
    PERCENTILES.map((p) => [p, percentile(tokenValues, p)])
  );

  const result = {
    packetsDir: PACKETS_DIR,
    fileCount: stats.length,
    tokens: {
      min,
      max,
      avg,
      percentiles,
    },
    perFile: stats,
  };

  console.log(JSON.stringify(result, null, 2));
}

main().catch((err) => {
  console.error(err);
  process.exit(1);
});