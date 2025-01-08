# Get List of availibility domains
data "oci_identity_availability_domains" "ADs" {
  compartment_id            = var.compartment_ocid
}

# Get a list of supported images based on the shape, operating system, and operating system version
data "oci_core_images" "compute_images" {
  compartment_id            = var.compartment_ocid
  operating_system          = var.image_os
  operating_system_version  = var.image_os_version
  shape                     = var.instance_shape
  sort_by                   = "TIMECREATED"
  sort_order                = "DESC"
}