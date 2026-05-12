export interface Property {
  id: string;
  userId: string;
  categoryId: string;
  title: string;
  description?: string;
  price: number;
  pricePerM2?: number;
  area: number;
  address: string;
  ward?: string;
  district?: string;
  city: string;
  projectName?: string;
  status: string;
  expiredAt?: string;
  listingCode?: string;
  listingType?: string;
  bedrooms?: number;
  bathrooms?: number;
  images?: PropertyImage[];
  createdAt: string;
  updatedAt: string;
}

export interface CreatePropertyRequest {
  categoryId: string;
  title: string;
  description?: string;
  price: number;
  pricePerM2?: number;
  area: number;
  address: string;
  ward?: string;
  district?: string;
  city: string;
  projectName?: string;
  status?: string;
  expiredAt?: string;
  listingCode?: string;
  listingType?: string;
  bedrooms?: number;
  bathrooms?: number;
}

export interface UpdatePropertyRequest {
  categoryId: string;
  title: string;
  description?: string;
  price: number;
  pricePerM2?: number;
  area: number;
  address: string;
  ward?: string;
  district?: string;
  city: string;
  projectName?: string;
  status?: string;
  expiredAt?: string;
  listingCode?: string;
  listingType?: string;
  bedrooms?: number;
  bathrooms?: number;
}

export interface PropertyQueryParams {
  keyword?: string;
  city?: string;
  categoryId?: string;
  categoryGroup?: string;
  minPrice?: number;
  maxPrice?: number;
  minArea?: number;
  maxArea?: number;
  status?: string;
  page?: number;
  pageSize?: number;
}

export interface PropertyImage {
  id: string;
  propertyId: string;
  imageUrl: string;
  isPrimary: boolean;
  createdAt: string;
}
