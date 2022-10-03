import { createContext } from "react";
import ArticleStore from "./ArticleStore";

export const rootStoreContext = createContext({
  articleStore: new ArticleStore(),
});
