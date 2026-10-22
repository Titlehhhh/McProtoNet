import type {ReactNode} from 'react';
import clsx from 'clsx';
import Link from '@docusaurus/Link';
import useDocusaurusContext from '@docusaurus/useDocusaurusContext';
import Layout from '@theme/Layout';
import Heading from '@theme/Heading';
import CodeBlock from '@theme/CodeBlock';
import Translate, {translate} from '@docusaurus/Translate';
import useBaseUrl from '@docusaurus/useBaseUrl';

import styles from './index.module.css';

/**
 * Главная. Скелет — из разбора живых сайтов документации библиотек:
 * на первом экране всегда либо код, либо команда установки, никогда не
 * пустой hero с одной кнопкой. Тексты взяты из content/ru/index.md.
 */

const INSTALL = 'dotnet add package McProtoNet --prerelease';

const SAMPLE = `using McProtoNet;
using McProtoNet.Protocol;
using HandshakeSb = McProtoNet.Protocol.Packets.Handshaking.Serverbound;
using LoginSb = McProtoNet.Protocol.Packets.Login.Serverbound;

const int Pv = 772;                                    // 1.21.8

await using var client = new MinecraftClient(
    new MinecraftClientOptions { Host = "127.0.0.1", Port = 25565 });
await client.ConnectAsync();

await client.SendAsync(
    new HandshakeSb.SetProtocolPacket(Pv, "127.0.0.1", 25565, 2), Pv);
await client.SendAsync(
    new LoginSb.LoginStartPacket("McProtoBot", V764_Last: new(Guid.NewGuid())),
    Pv);

var bot = new Bot(client, Pv);                         // the class from First bot

await foreach (var packet in client.ReadPacketsAsync())
    await bot.HandleAsync(in packet, Pv);`;

const OUTPUT = `health 20, food 20
health 20, food 19
health 18, food 19`;

type Feature = {id: string; title: string; body: string};

function Hero() {
  const {siteConfig} = useDocusaurusContext();
  // Фон - крупный кусок постройки, а не вся надпись: слово McProtoNet уже
  // стоит заголовком, второй раз читать его под ним незачем. Картинка
  // отдаётся в CSS через переменную, чтобы слой с размытием жил в ::before.
  const backdrop = useBaseUrl('/img/formation.png');
  return (
    <header
      className={clsx('hero', styles.hero)}
      style={{['--hero-backdrop' as string]: `url(${backdrop})`}}>
      <div className="container">
        <img
          className={styles.heroIcon}
          src={useBaseUrl('/img/icon.png')}
          alt={translate({
            id: 'home.hero.iconAlt',
            message: 'The McProtoNet package icon on nuget.org',
          })}
          width={96}
          height={96}
        />
        <Heading as="h1" className={styles.heroTitle}>
          {siteConfig.title}
        </Heading>
        <p className={styles.heroSubtitle}>{siteConfig.tagline}</p>
        <div className={styles.buttons}>
          <Link
            className="button button--primary button--lg"
            to="/docs/getting-started/first-bot">
            <Translate id="home.cta.tutorial">First bot</Translate>
          </Link>
          <Link
            className="button button--secondary button--lg"
            to="https://github.com/Titlehhhh/McProtoNet">
            <Translate id="home.cta.github">GitHub</Translate>
          </Link>
        </div>
        <div className={styles.install}>
          <CodeBlock language="bash" className={styles.installCode}>
            {INSTALL}
          </CodeBlock>
          <p className={styles.installNote}>
            <Translate id="home.install.note">
              .NET 8 or newer is required. There is no stable 2.0 yet, hence
              --prerelease.
            </Translate>
          </p>
          <p className={styles.backdropNote}>
            <Translate id="home.backdrop.note">
              The sign in the background was built by 117 bots on a chat
              command.
            </Translate>{' '}
            <Link to="https://github.com/Titlehhhh/McProtoNet/tree/dev/examples/FormationBots">
              <Translate id="home.backdrop.link">
                The FormationBots example
              </Translate>
            </Link>
          </p>
        </div>
      </div>
    </header>
  );
}

function Sample() {
  return (
    <section className={styles.section}>
      <div className="container">
        <div className={styles.sampleGrid}>
          <div className={styles.sampleCode}>
            <CodeBlock language="csharp" title="Program.cs">
              {SAMPLE}
            </CodeBlock>
          </div>
          <div className={styles.sampleOut}>
            <CodeBlock
              title={translate({id: 'home.sample.output', message: 'Output'})}>
              {OUTPUT}
            </CodeBlock>
            <p className={styles.sampleNote}>
              <Translate id="home.sample.note">
                The step by step walkthrough is in First bot.
              </Translate>
            </p>
          </div>
        </div>
      </div>
    </section>
  );
}

function useFeatures(): Feature[] {
  return [
    {
      id: 'protocol',
      title: translate({
        id: 'home.feature.protocol.title',
        message: 'The protocol, not the game',
      }),
      body: translate({
        id: 'home.feature.protocol.body',
        message:
          'Connect to a server, send and receive typed packets, turn on compression and encryption. A base for bots, custom clients and tools.',
      }),
    },
    {
      id: 'transport',
      title: translate({
        id: 'home.feature.transport.title',
        message: 'Fast transport',
      }),
      body: translate({
        id: 'home.feature.transport.body',
        message:
          'Compression through libdeflate, hardware AES for the cipher, parsing over Span<byte> without extra copies. NativeAOT compatible.',
      }),
    },
    {
      id: 'offline',
      title: translate({
        id: 'home.feature.offline.title',
        message: 'Offline servers',
      }),
      body: translate({
        id: 'home.feature.offline.body',
        message:
          'Handshake and login against servers in offline mode. The join order stays in the application code, so a non-standard server is not a dead end.',
      }),
    },
    {
      id: 'network',
      title: translate({
        id: 'home.feature.network.title',
        message: 'Network helpers',
      }),
      body: translate({
        id: 'home.feature.network.body',
        message: 'SRV record lookup and LAN server discovery.',
      }),
    },
  ];
}

function Features() {
  const features = useFeatures();
  return (
    <section className={styles.section}>
      <div className="container">
        <div className={styles.cards}>
          {features.map((f) => (
            <div key={f.id} className={styles.card}>
              <Heading as="h2" className={styles.cardTitle}>
                {f.title}
              </Heading>
              <p className={styles.cardBody}>{f.body}</p>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
}

function Multiversion() {
  return (
    <section className={clsx(styles.section, styles.band)}>
      <div className="container">
        <Heading as="h2" className={styles.bandTitle}>
          <Translate id="home.mv.title">One build, many versions</Translate>
        </Heading>
        <p className={styles.bandRange}>
          1.16 <span className={styles.bandDash}>—</span> 26.2
        </p>
        <p className={styles.bandNote}>
          <Translate id="home.mv.note">
            Protocols 735-776. The version is given at connection time, and
            nothing has to be rebuilt for it.
          </Translate>
        </p>
      </div>
    </section>
  );
}

function Packages() {
  return (
    <section className={styles.section}>
      <div className="container">
        <Heading as="h2" className={styles.sectionTitle}>
          <Translate id="home.packages.title">Packages</Translate>
        </Heading>
        <p className={styles.tableNote}>
          <Translate id="home.packages.note">
            The library is split into five NuGet packages. Only McProtoNet is
            installed; the rest come with it.
          </Translate>{' '}
          <Link to="/docs/getting-started/installation">
            <Translate id="home.packages.link">
              What is inside each one is in Installation
            </Translate>
          </Link>
        </p>
      </div>
    </section>
  );
}

function Next() {
  return (
    <section className={clsx(styles.section, styles.next)}>
      <div className="container">
        <Heading as="h2" className={styles.sectionTitle}>
          <Translate id="home.next.title">Where to go next</Translate>
        </Heading>
        <ul className={styles.nextList}>
          <li>
            <Link to="/docs/getting-started/first-bot">
              <Translate id="home.next.firstBot.link">First bot</Translate>
            </Link>{' '}
            <Translate id="home.next.firstBot.note">
              - installation, the protocol version, four phases, a bot that
              works.
            </Translate>
          </li>
          <li>
            <Link to="/docs/overview/non-goals">
              <Translate id="home.next.nonGoals.link">
                What the library does not do
              </Translate>
            </Link>{' '}
            <Translate id="home.next.nonGoals.note">
              - worth reading before a bot is planned.
            </Translate>
          </li>
          <li>
            <Link to="/docs/reference/glossary">
              <Translate id="home.next.glossary.link">Glossary</Translate>
            </Link>{' '}
            <Translate id="home.next.glossary.note">
              - frame, phase, ordinal and other words from the API.
            </Translate>
          </li>
        </ul>
      </div>
    </section>
  );
}

export default function Home(): ReactNode {
  return (
    <Layout
      description={translate({
        id: 'home.description',
        message:
          'An open C# library for the Minecraft Java Edition protocol: one build works on versions 1.16-26.2.',
      })}>
      <Hero />
      <main>
        <Sample />
        <Features />
        <Multiversion />
        <Packages />
        <Next />
      </main>
    </Layout>
  );
}
