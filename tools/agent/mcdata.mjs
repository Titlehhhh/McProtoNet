import { promises as fs } from 'node:fs'
import path from 'node:path'
import { encodeLines } from '@toon-format/toon'

const rootDir = path.resolve('src', 'McProtoNet.Protocol', 'minecraft-data')
const typesDir = path.join(rootDir, 'types')
const encodeOptions = { delimiter: ',', keyFolding: 'safe' }

process.stdout.on('error', (error) => {
  if (error && error.code === 'EPIPE') {
    process.exit(0)
  }
  throw error
})

function outputToon(value) {
  for (const line of encodeLines(value, encodeOptions)) {
    process.stdout.write(`${line}\n`)
  }
}

function fail(message) {
  console.error(message)
  process.exit(1)
}

function parseArgs(argv) {
  const opts = {}
  const positional = []
  for (let i = 0; i < argv.length; i += 1) {
    const arg = argv[i]
    if (arg.startsWith('--')) {
      const key = arg.slice(2)
      const next = argv[i + 1]
      if (next && !next.startsWith('--')) {
        opts[key] = next
        i += 1
      } else {
        opts[key] = true
      }
    } else {
      positional.push(arg)
    }
  }
  return { opts, positional }
}

function parseProtocolVersion(raw) {
  if (!raw) return null
  if (!/^\d+$/.test(raw)) {
    fail('Version must be a protocol number (integer).')
  }
  return Number(raw)
}

function parseRange(rangeKey) {
  const parts = rangeKey.split('-')
  if (parts.length === 1) {
    const value = Number(parts[0])
    return { start: value, end: value }
  }
  const start = Number(parts[0])
  const end = Number(parts[1])
  return { start, end }
}

function sortRanges(keys) {
  return keys.slice().sort((a, b) => {
    const ra = parseRange(a)
    const rb = parseRange(b)
    if (ra.start !== rb.start) return ra.start - rb.start
    return ra.end - rb.end
  })
}

function selectHistoryEntry(history, version) {
  const ranges = Object.keys(history)
  for (const rangeKey of ranges) {
    const { start, end } = parseRange(rangeKey)
    if (version >= start && version <= end) {
      return { range: rangeKey, entry: history[rangeKey] }
    }
  }
  return null
}

function mergeHistory(histories) {
  const segments = []
  for (const history of histories) {
    if (!history) continue
    for (const [rangeKey, entry] of Object.entries(history)) {
      const { start, end } = parseRange(rangeKey)
      segments.push({ start, end, value: entry })
    }
  }
  if (segments.length === 0) return {}

  const boundaries = new Set()
  for (const segment of segments) {
    boundaries.add(segment.start)
    boundaries.add(segment.end + 1)
  }
  const sorted = Array.from(boundaries).sort((a, b) => a - b)

  const resolved = []
  for (let i = 0; i < sorted.length - 1; i += 1) {
    const segStart = sorted[i]
    const segEnd = sorted[i + 1] - 1
    const covering = segments.filter(
      (segment) => segment.start <= segStart && segment.end >= segEnd,
    )
    if (covering.length === 0) continue
    const nonNull = covering.filter((segment) => segment.value !== null)
    if (nonNull.length > 1) {
      fail(`History conflict on range ${segStart}-${segEnd}`)
    }
    const value = nonNull.length === 1 ? nonNull[0].value : null
    resolved.push({ start: segStart, end: segEnd, value })
  }

  const coalesced = []
  for (const segment of resolved) {
    const valueKey =
      segment.value === null ? 'null' : JSON.stringify(segment.value)
    const last = coalesced[coalesced.length - 1]
    if (last && last.valueKey === valueKey && last.end + 1 === segment.start) {
      last.end = segment.end
      continue
    }
    coalesced.push({
      start: segment.start,
      end: segment.end,
      value: segment.value,
      valueKey,
    })
  }

  const merged = {}
  for (const segment of coalesced) {
    const rangeKey =
      segment.start === segment.end
        ? `${segment.start}`
        : `${segment.start}-${segment.end}`
    merged[rangeKey] = segment.value
  }

  return merged
}

async function readJson(filePath) {
  const raw = await fs.readFile(filePath, 'utf8')
  return JSON.parse(raw)
}

async function readInfoBounds() {
  const infoPath = path.join(rootDir, 'info.txt')
  const raw = await fs.readFile(infoPath, 'utf8')
  const startMatch = raw.match(/startVersion=(\d+)/)
  const latestMatch = raw.match(/latestVersion=(\d+)/)
  if (!startMatch || !latestMatch) {
    return null
  }
  return {
    start: Number(startMatch[1]),
    end: Number(latestMatch[1]),
  }
}

async function listDirs(dir) {
  const entries = await fs.readdir(dir, { withFileTypes: true })
  return entries.filter((entry) => entry.isDirectory()).map((entry) => entry.name)
}

async function listFiles(dir) {
  const entries = await fs.readdir(dir, { withFileTypes: true })
  return entries.filter((entry) => entry.isFile()).map((entry) => entry.name)
}

async function buildTypeIndex() {
  const files = await listFiles(typesDir)
  const index = new Map()
  for (const file of files) {
    if (!file.endsWith('.json')) continue
    const base = file.replace(/\.json$/i, '')
    const canonical = base.endsWith('_1') ? base.slice(0, -2) : base
    if (!index.has(canonical)) index.set(canonical, [])
    index.get(canonical).push(path.join(typesDir, file))
  }
  return index
}

async function loadType(typeName) {
  const index = await buildTypeIndex()
  const files = index.get(typeName)
  if (!files || files.length === 0) {
    fail(`Type not found: ${typeName}`)
  }
  const payloads = []
  for (const filePath of files) {
    payloads.push(await readJson(filePath))
  }
  const mergedHistory = mergeHistory(payloads.map((p) => p.history))
  const base = { ...payloads[0], name: typeName, history: mergedHistory }
  if (files.length > 1) {
    base.aliases = files.map((filePath) =>
      path.basename(filePath, '.json'),
    )
  }
  return base
}

async function loadPacket(state, direction, packetName) {
  const filename = packetName.endsWith('.json')
    ? packetName
    : `${packetName}.json`
  const packetPath = path.join(rootDir, state, direction, filename)
  try {
    return await readJson(packetPath)
  } catch {
    fail(`Packet not found: ${state}/${direction}/${filename}`)
  }
}

function containsFieldName(value, fieldName) {
  if (Array.isArray(value)) {
    return value.some((item) => containsFieldName(item, fieldName))
  }
  if (value && typeof value === 'object') {
    if (value.name === fieldName) return true
    return Object.values(value).some((item) =>
      containsFieldName(item, fieldName),
    )
  }
  return false
}

async function commandList(opts) {
  if (opts.roots) {
    const roots = await listDirs(rootDir)
    outputToon({ roots })
    return
  }
  if (opts.states) {
    const states = (await listDirs(rootDir)).filter((name) => name !== 'types')
    outputToon({ states })
    return
  }
  if (opts.directions) {
    if (!opts.state) fail('Missing --state for directions list.')
    const directions = await listDirs(path.join(rootDir, opts.state))
    outputToon({ state: opts.state, directions })
    return
  }
  if (opts.packets) {
    if (!opts.state || !opts.direction) {
      fail('Missing --state or --direction for packets list.')
    }
    const packetDir = path.join(rootDir, opts.state, opts.direction)
    const files = await listFiles(packetDir)
    const packets = files
      .filter((file) => file.startsWith('Packet') && file.endsWith('.json'))
      .filter((file) => file !== 'Packet.json')
      .map((file) => file.replace(/\.json$/i, ''))
    outputToon({ state: opts.state, direction: opts.direction, packets })
    return
  }
  if (opts.types) {
    const index = await buildTypeIndex()
    const types = Array.from(index.keys()).sort()
    outputToon({ types })
    return
  }
  fail('list requires one of --roots, --states, --directions, --packets, --types')
}

async function commandSchema(opts) {
  const version = parseProtocolVersion(opts.version)
  if (opts.type) {
    const typeData = await loadType(opts.type)
    if (!version) {
      outputToon(typeData)
      return
    }
    const match = selectHistoryEntry(typeData.history, version)
    if (!match || match.entry === null) {
      fail(`Type ${opts.type} is not available in ${version}.`)
    }
    outputToon({
      name: typeData.name,
      version,
      range: match.range,
      schema: match.entry,
    })
    return
  }
  if (opts.packet) {
    if (!opts.state || !opts.direction) {
      fail('Missing --state or --direction for packet schema.')
    }
    const packetData = await loadPacket(
      opts.state,
      opts.direction,
      opts.packet,
    )
    if (!version) {
      outputToon(packetData)
      return
    }
    const match = selectHistoryEntry(packetData.history, version)
    if (!match || match.entry === null) {
      fail(`Packet ${opts.packet} is not available in ${version}.`)
    }
    outputToon({
      name: packetData.name ?? opts.packet,
      version,
      range: match.range,
      schema: match.entry,
    })
    return
  }
  fail('schema requires --type or --packet')
}

async function commandHistory(opts) {
  const includeMissing = Boolean(opts['include-missing'])

  const buildRanges = async (history) => {
    const segments = Object.keys(history).map((range) => {
      const { start, end } = parseRange(range)
      return { start, end, exists: history[range] !== null }
    })
    segments.sort((a, b) => (a.start !== b.start ? a.start - b.start : a.end - b.end))

    let expanded = segments
    if (includeMissing) {
      const bounds = await readInfoBounds()
      if (!bounds) {
        fail('Failed to read info.txt for include-missing ranges.')
      }
      const filled = []
      let cursor = bounds.start
      for (const seg of segments) {
        if (seg.end < bounds.start || seg.start > bounds.end) continue
        const segStart = Math.max(seg.start, bounds.start)
        const segEnd = Math.min(seg.end, bounds.end)
        if (segStart > cursor) {
          filled.push({ start: cursor, end: segStart - 1, exists: false })
        }
        filled.push({ start: segStart, end: segEnd, exists: seg.exists })
        cursor = segEnd + 1
      }
      if (cursor <= bounds.end) {
        filled.push({ start: cursor, end: bounds.end, exists: false })
      }
      expanded = filled
    }

    const merged = []
    for (const seg of expanded) {
      const last = merged[merged.length - 1]
      if (last && last.exists === seg.exists && last.end + 1 === seg.start) {
        last.end = seg.end
        continue
      }
      merged.push({ ...seg })
    }

    return merged.map((seg) => ({
      range: seg.start === seg.end ? `${seg.start}` : `${seg.start}-${seg.end}`,
      exists: seg.exists,
    }))
  }

  if (opts.type) {
    const typeData = await loadType(opts.type)
    const ranges = await buildRanges(typeData.history)
    outputToon({ name: typeData.name, ranges })
    return
  }
  if (opts.packet) {
    if (!opts.state || !opts.direction) {
      fail('Missing --state or --direction for packet history.')
    }
    const packetData = await loadPacket(
      opts.state,
      opts.direction,
      opts.packet,
    )
    const ranges = await buildRanges(packetData.history)
    outputToon({ name: packetData.name ?? opts.packet, ranges })
    return
  }
  fail('history requires --type or --packet')
}

async function commandFind(opts) {
  if (!opts.field && !opts.type && !opts.packet) {
    fail('find requires --field, --type, or --packet')
  }
  const matches = []
  if (opts.type) {
    const index = await buildTypeIndex()
    const query = opts.type.toLowerCase()
    for (const typeName of index.keys()) {
      if (typeName.toLowerCase().includes(query)) {
        matches.push({
          path: path.relative(rootDir, path.join('types', `${typeName}.json`)),
          name: typeName,
        })
      }
    }
  }
  if (opts.packet) {
    const query = opts.packet.toLowerCase()
    const states = (await listDirs(rootDir)).filter((name) => name !== 'types')
    for (const state of states) {
      const directions = await listDirs(path.join(rootDir, state))
      for (const direction of directions) {
        const files = await listFiles(path.join(rootDir, state, direction))
        for (const file of files) {
          if (!file.startsWith('Packet') || !file.endsWith('.json')) continue
          if (file === 'Packet.json') continue
          if (!file.toLowerCase().includes(query)) continue
          matches.push({
            path: path.relative(
              rootDir,
              path.join(state, direction, file),
            ),
            name: file.replace(/\.json$/i, ''),
          })
        }
      }
    }
  }
  if (opts.field) {
    const fieldName = opts.field
    const states = (await listDirs(rootDir)).filter((name) => name !== 'types')
    for (const [typeName, files] of (await buildTypeIndex()).entries()) {
      for (const filePath of files) {
        const data = await readJson(filePath)
        if (containsFieldName(data, fieldName)) {
          matches.push({
            path: path.relative(rootDir, filePath),
            name: typeName,
          })
          break
        }
      }
    }
    for (const state of states) {
      const directions = await listDirs(path.join(rootDir, state))
      for (const direction of directions) {
        const files = await listFiles(path.join(rootDir, state, direction))
        for (const file of files) {
          if (!file.endsWith('.json')) continue
          const data = await readJson(path.join(rootDir, state, direction, file))
          if (containsFieldName(data, fieldName)) {
            matches.push({
              path: path.relative(
                rootDir,
                path.join(state, direction, file),
              ),
              name: file.replace(/\.json$/i, ''),
            })
          }
        }
      }
    }
  }
  outputToon({ matches })
}

function normalizeDelimiter(raw) {
  if (!raw) return ','
  if (raw === '\\t' || raw === 'tab') return '\t'
  if (raw === 'pipe' || raw === '|') return '|'
  if (raw === 'comma' || raw === ',') return ','
  if (raw.length === 1) return raw
  fail('Unsupported delimiter. Use comma, tab, or pipe.')
  return ','
}

async function main() {
  const { opts, positional } = parseArgs(process.argv.slice(2))
  encodeOptions.delimiter = normalizeDelimiter(opts.delimiter)
  const command = positional[0]
  if (!command) {
    fail('Missing command. Use list, schema, history, or find.')
  }
  if (command === 'list') {
    await commandList(opts)
    return
  }
  if (command === 'schema') {
    await commandSchema(opts)
    return
  }
  if (command === 'history') {
    await commandHistory(opts)
    return
  }
  if (command === 'find') {
    await commandFind(opts)
    return
  }
  fail(`Unknown command: ${command}`)
}

await main()
