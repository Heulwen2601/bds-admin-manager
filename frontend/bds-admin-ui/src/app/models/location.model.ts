export interface Location {
  id: string;
  name: string;
  type: 'country' | 'city' | 'district' | 'ward';
  parentId?: string;
  children?: Location[];
}