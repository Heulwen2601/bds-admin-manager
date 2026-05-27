export interface Lead {
  id: string;
  propertyId: string;
  propertyTitle?: string;
  propertyListingCode?: string;
  propertyStatus?: string;
  isRead: boolean;
  readAt?: string;
  userId?: string;
  fullName?: string;
  phone?: string;
  email?: string;
  guestName?: string;
  guestPhone?: string;
  guestEmail?: string;
  message?: string;
  status?: string;
  customerProfile?: LeadCustomerProfile;
  createdAt: string;
}

export interface LeadCustomerProfile {
  id: string;
  fullName: string;
  email: string;
  phone?: string;
  role: string;
  createdAt: string;
}

export interface CreateLeadRequest {
  fullName: string;
  phone: string;
  email?: string;
  message?: string;
  guestName?: string;
  guestPhone?: string;
  guestEmail?: string;
}
