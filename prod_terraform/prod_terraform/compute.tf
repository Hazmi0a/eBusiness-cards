# Compute Instances
# data "templatefile" "user_data" {
#   template = file("user_data.sh")
# }

resource "oci_core_instance" "web-01" {
  availability_domain   = data.oci_identity_availability_domains.ADs.availability_domains[0].name
    compartment_id      = var.compartment_ocid
    display_name        = "web-Server-01"
    shape               = var.instance_shape
    shape_config {
        #Optional
        # baseline_ocpu_utilization = var.instance_shape_config_baseline_ocpu_utilization
        memory_in_gbs = "4"
        # nvmes = var.instance_shape_config_nvmes
        ocpus = "2"
        # vcpus = var.instance_shape_config_vcpus
    }

    create_vnic_details {
        assign_public_ip = true
        display_name     = "web-Server-01"
        subnet_id        = oci_core_subnet.subnet.id
    }

    source_details {
        source_type                 = "image"
        source_id                   = data.oci_core_images.compute_images.images[0].id
        boot_volume_size_in_gbs     = "50"
    }

    metadata =  {
        ssh_authorized_keys = "${var.hazmi_ssh_public_key}\n${var.younes_ssh_public_key}\n${var.ahmed_ssh_public_key}\n${var.ansible_ssh_public_key}"
        # user_data           = data.template_file.user_data.rendered
    }

}