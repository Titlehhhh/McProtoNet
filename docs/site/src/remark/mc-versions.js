import {visit} from 'unist-util-visit';

/**
 * Подстановка переменных вида %имя% в тексте страниц.
 *
 * Наследство Writerside: в старых страницах диапазон версий записан как
 * %min_minecraft_version% / %max_minecraft_version%. Docusaurus таких
 * подстановок не умеет, поэтому меняем их здесь, до рендера.
 *
 * Значения — из src/McProtoNet.Protocol/MinecraftVersion.cs: нижняя
 * объявленная версия и верхняя.
 */
export const variables = {
  min_minecraft_version: '1.16',
  max_minecraft_version: '26.2',
};

const pattern = /%([a-z0-9_]+)%/gi;

export default function remarkMcVersions() {
  return (tree, file) => {
    visit(tree, ['text', 'inlineCode', 'code'], (node) => {
      if (typeof node.value !== 'string' || !node.value.includes('%')) {
        return;
      }
      node.value = node.value.replace(pattern, (match, name) => {
        const value = variables[name];
        if (value === undefined) {
          file.message(`Неизвестная переменная ${match}`, node);
          return match;
        }
        return value;
      });
    });
  };
}
