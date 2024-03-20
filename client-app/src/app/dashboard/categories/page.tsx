"use client";
import { Category } from "@/types/category/category";
import { BaseListResponse } from "@/types/common/baseListResponse";
import { BaseRequest } from "@/types/common/baseRequest";
import axiosApiInstance from "@/utils/axiosApiInstance";
import React, { useEffect, useState } from "react";

export async function getCategories(params: BaseRequest) {
  try {
    const categoryList = await axiosApiInstance.get<BaseListResponse<Category>>(
      "api/category",
      {
        params: params,
      }
    );
    return categoryList.data;
  } catch (error) {}
}

function CategoryPage() {
  const [categoryList, setCategoryList] =
    useState<BaseListResponse<Category>>();

  useEffect(() => {
    // declare the data fetching function
    const fetchData = async () => {
      const data = await getCategories({ pageIndex: 0, pageSize: 10 });
      setCategoryList(data);
    };

    // call the function
    fetchData();
  }, []);

  return (
    <>
      <p>Kategoriler</p>
      <ul>
        {categoryList?.items.map((item, index) => {
          return <li key={index}>{item.name}</li>;
        })}
      </ul>
    </>
  );
}

export default CategoryPage;
