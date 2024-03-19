import { Category } from "@/types/category/category";
import { BaseListResponse } from "@/types/common/baseListResponse";
import { BaseRequest } from "@/types/common/baseRequest";
import axiosApiInstance from "@/utils/axiosApiInstance";

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
