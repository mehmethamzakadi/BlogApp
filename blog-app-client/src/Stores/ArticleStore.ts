import { action, makeObservable, observable } from "mobx";

export default class ArticleStore {
  articleTitle = "Başlık";
  constructor() {
    makeObservable(this, {
      articleTitle: observable,
      setArticle: action,
    });
  }

  setArticle = (title: string) => {
    this.articleTitle = title;
    return this.articleTitle;
  };
}
