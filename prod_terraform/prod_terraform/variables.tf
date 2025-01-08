variable "tenancy_ocid" {
  description = "OCID of the tenancy"
  sensitive = true
}
variable "user_ocid" {
  description = "OCID of the user"
  sensitive = true
}
variable "fingerprint" {
  description   = "Fingerprint of the user's public key"
  sensitive = true
}
variable "region" {
  default = "me-jeddah-1"
}
variable "private_key_path" {
  description = "Path to the private key"
}
variable "compartment_ocid" {
  description = "OCID of the compartment"
  sensitive = true
}
variable "ssh_public_key" {
}
variable "AD" {
  description = "Availability Domain"
  default = "1"
}
variable "vcn_cidr" {
  description = "CIDR block for the VCN"
  default = "10.0.0.0/16"
}
variable "vcn_dns_label" {
    description = "DNS label for the VCN"
    default = "vcn01"
}
variable "dns_label" {
  description = "DNS label for the subnet"
  default = "public"
}

# OS Image
variable "image_os" {
  description = "Operating System"
  default = "Oracle Linux"
}
variable "image_os_version" {
  description = "Operating System Version"
  default = "8"
}

variable "instance_shape" {
  description = "Instance Shape"
  default = "VM.Standard.E4.Flex"
}
  
variable "load_balancer_min_band" {
    description = "Minimum Bandwidth"
    default = "10"
}  
variable "load_balancer_max_band" {
    description = "Maximum Bandwidth"
    default = "10"
}

variable "path_local_public_key" {
  description = "Path to local public key"
  sensitive = true
}
variable "younes_ssh_public_key" {
  description = "Path to local public key"
  sensitive = true
}
variable "ahmed_ssh_public_key" {
  description = "Path to local public key"
  sensitive = true
}
variable "hazmi_ssh_public_key" {
  description = "Path to local public key"
  sensitive = true
}
variable "ansible_ssh_public_key" {
  description = "Path to local public key"
  sensitive = true
}
