export interface Category {
  id: string;
  name: string;
  description?: string;
  parentId?: string;
  children?: Category[];
  groupName: string;
  slug: string;
}

export interface CreateCategoryRequest {
  name: string;
  description?: string;
  parentId?: string;
  groupName: string;
  slug: string;
}

export interface UpdateCategoryRequest {
  name: string;
  description?: string;
  parentId?: string;
  groupName: string;
  slug: string;
}