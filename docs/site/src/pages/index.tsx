import type {ReactNode} from 'react';
import clsx from 'clsx';
import Link from '@docusaurus/Link';
import useDocusaurusContext from '@docusaurus/useDocusaurusContext';
import Layout from '@theme/Layout';
import Heading from '@theme/Heading';
import CodeBlock from '@theme/CodeBlock';
import Translate, {translate} from '@docusaurus/Translate';

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

await client.SendAsync(new HandshakeSb.SetProtocolPacket(Pv, "127.0.0.1", 25565, 2), Pv);
await client.SendAsync(new LoginSb.LoginStartPacket("McProtoBot", V764_Last: new(Guid.NewGuid())), Pv);

await foreach (var packet in client.ReadPacketsAsync())
    await bot.HandleAsync(in packet, Pv);`;

const OUTPUT = `здоровье 20, еда 20
здоровье 20, еда 19
здоровье 18, еда 19`;

type Feature = {title: string; body: string};

const FEATURES: Feature[] = [
  {
    title: 'Протокол, а не игра',
    body: 'Подключиться к серверу, отправлять и принимать типизированные пакеты, включать сжатие и шифрование. Основа для ботов, своих клиентов и инструментов.',
  },
  {
    title: 'Много версий, один API',
    body: 'Каждый пакет знает свою раскладку по версиям. Версию протокола вы выбираете один раз, при подключении.',
  },
  {
    title: 'Оффлайн-серверы',
    body: 'Handshake и login на серверы в offline-режиме из коробки. Машина состояний входа остаётся в вашем коде, поэтому нестандартные серверы — не тупик.',
  },
  {
    title: 'Сетевые помощники',
    body: 'Поиск SRV-записей и обнаружение LAN-серверов.',
  },
];

const PACKAGES: [string, string][] = [
  ['McProtoNet', 'Клей: MinecraftClient, типизированный SendAsync<T>, SRV, LAN'],
  ['McProtoNet.Transport', 'Байты между сокетом и пакетом: кадры, сжатие, шифрование'],
  ['McProtoNet.Protocol', 'Пакетный слой: классы пакетов, реестр, диспетчер, базы обработчиков'],
  ['McProtoNet.Primitives', 'Примитивный читатель и писатель, буферы, IncomingPacket / OutgoingPacket'],
  ['McProtoNet.NBT', 'Разбор и запись NBT'],
];

function Hero() {
  const {siteConfig} = useDocusaurusContext();
  return (
    <header className={clsx('hero', styles.hero)}>
      <div className="container">
        <Heading as="h1" className={styles.heroTitle}>
          {siteConfig.title}
        </Heading>
        <p className={styles.heroSubtitle}>{siteConfig.tagline}</p>
        <div className={styles.buttons}>
          <Link
            className="button button--primary button--lg"
            to="/docs/getting-started/minimal-bot">
            <Translate id="home.cta.tutorial">Первый бот</Translate>
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
              Нужен .NET 8 или новее. Стабильного 2.0 пока нет, поэтому
              --prerelease.
            </Translate>
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
            <CodeBlock title="Вывод">{OUTPUT}</CodeBlock>
            <p className={styles.sampleNote}>
              <Translate id="home.sample.note">
                Полный разбор по шагам — в «Первом боте».
              </Translate>
            </p>
          </div>
        </div>
      </div>
    </section>
  );
}

function Features() {
  return (
    <section className={styles.section}>
      <div className="container">
        <div className={styles.cards}>
          {FEATURES.map((f) => (
            <div key={f.title} className={styles.card}>
              <Heading as="h3" className={styles.cardTitle}>
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
          <Translate id="home.mv.title">Одна сборка — много версий</Translate>
        </Heading>
        <p className={styles.bandRange}>
          1.16 <span className={styles.bandDash}>—</span> 26.2
        </p>
        <p className={styles.bandNote}>
          <Translate id="home.mv.note">
            Протоколы 735–776. Версию вы называете при подключении, пересобирать
            под неё ничего не нужно.
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
          <Translate id="home.packages.title">NuGet-пакеты</Translate>
        </Heading>
        <table className={styles.table}>
          <thead>
            <tr>
              <th>
                <Translate id="home.packages.col1">Пакет</Translate>
              </th>
              <th>
                <Translate id="home.packages.col2">Что внутри</Translate>
              </th>
            </tr>
          </thead>
          <tbody>
            {PACKAGES.map(([name, what]) => (
              <tr key={name}>
                <td>
                  <code>{name}</code>
                </td>
                <td>{what}</td>
              </tr>
            ))}
          </tbody>
        </table>
        <p className={styles.tableNote}>
          <Translate id="home.packages.note">
            Ставится один McProtoNet — остальные приезжают вместе с ним.
          </Translate>
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
          <Translate id="home.next.title">Куда дальше</Translate>
        </Heading>
        <ul className={styles.nextList}>
          <li>
            <Link to="/docs/getting-started/minimal-bot">Первый бот</Link> —
            установка, версия протокола, четыре фазы, работающий бот.
          </li>
          <li>
            <Link to="/docs/overview/non-goals">Что библиотека не делает</Link> —
            прочитайте до того, как планировать бота.
          </li>
          <li>
            <Link to="/docs/reference/glossary">Словарь</Link> — кадр, фаза,
            ординал и другие слова из API.
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
          'Библиотека на C# для протокола Minecraft Java Edition: одна сборка на версии 1.16–26.2, сторона клиента.',
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
