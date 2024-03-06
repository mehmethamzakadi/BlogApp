"use client";
import { getCategories } from "@/services/categoryService";
import { Category } from "@/types/category/category";
import { BaseRequest } from "@/types/common/baseRequest";
import React, { useState } from "react";

async function CategoryPage() {
  debugger;
  const [categoryList, setCategoryList] = useState<Category[]>();

  await getCategories({ pageIndex: 0, pageSize: 10 }).then((x) =>
    setCategoryList(x?.items)
  );
  return (
    <>
      <p>Kategoriler</p>
      <ul>
        {categoryList?.map((x) => (
          <li>{x.name}</li>
        ))}
      </ul>
    </>
  );
}

export default CategoryPage;
