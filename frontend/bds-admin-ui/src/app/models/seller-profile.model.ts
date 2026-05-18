import { LoginResponse } from './auth.model';

export type SellerType = 'Broker' | 'CompanyRepresentative' | 'Owner';

export interface SellerProfile {
  id: string;
  userId: string;
  sellerType: SellerType;
  sellerTypeName: string;
  companyName?: string;
  contactName: string;
  phone: string;
  address?: string;
  taxCode?: string;
  createdAt: string;
  updatedAt: string;
}

export interface BecomeSellerRequest {
  sellerType: SellerType;
  companyName?: string;
  contactName: string;
  phone: string;
  address?: string;
  taxCode?: string;
}

export interface BecomeSellerResponse {
  profile: SellerProfile;
  auth: LoginResponse;
}

export interface UpdateSellerProfileRequest {
  sellerType: SellerType;
  companyName?: string;
  contactName: string;
  phone: string;
  address?: string;
  taxCode?: string;
}

export interface SellerDirectoryQuery {
  type?: SellerType;
  keyword?: string;
  city?: string;
}

export interface SellerDirectoryProfile {
  id: string;
  userId: string;
  sellerType: SellerType;
  sellerTypeName: string;
  displayName: string;
  companyName?: string;
  phone?: string;
  email?: string;
  address?: string;
  listings: number;
  createdAt: string;
}
