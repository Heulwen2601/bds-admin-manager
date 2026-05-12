export interface SellerProfile {
  id: string;
  userId: string;
  businessName?: string;
  businessAddress?: string;
  businessPhone?: string;
  businessEmail?: string;
  licenseNumber?: string;
  description?: string;
  isVerified: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface BecomeSellerRequest {
  businessName?: string;
  businessAddress?: string;
  businessPhone?: string;
  businessEmail?: string;
  licenseNumber?: string;
  description?: string;
}

export interface UpdateSellerProfileRequest {
  businessName?: string;
  businessAddress?: string;
  businessPhone?: string;
  businessEmail?: string;
  licenseNumber?: string;
  description?: string;
}
