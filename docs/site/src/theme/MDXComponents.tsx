import MDXComponents from '@theme-original/MDXComponents';
import Tabs from '@theme/Tabs';
import TabItem from '@theme/TabItem';

// Чтобы <Tabs> и <TabItem> работали на любой странице без импорта в каждом
// файле. Имена обязаны быть с большой буквы, иначе MDX отрендерит их как
// обычные HTML-теги.
export default {
  ...MDXComponents,
  Tabs,
  TabItem,
};
