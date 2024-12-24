# Getting started with ansible

## installing ansible packages (ansible-galaxy) from requirements file

1- `ansible-galaxy install -r requirements.yml`

## steps to execute playbooks

1- while writing the cfg file. use the root user for server (opc in case of OCI) and a valid ssh private key
2- create ssh key pairs for ansible user to use `ssh-keygen -t ed25519 -C ansible`
3- run the bootstrapping yml to create ansible user with sudo privileges and add it ssh key. `ansible-playbook playbooks/bootstrapping.yml`

## deploying traefik reverse proxy

1- create a traefik dashboard password using htpasswd:
`htpasswd -nb admin <password>`
2- replace password inside `/traefik-data/configurations/dynamic.yml` under `basicAuth.users`
3- deploy traefik using `ansible-playbook playbooks/traefik_reverse_proxy.yml`

## deploy apps

deploy using `ansible-playbook playbooks/deploy-apps.yml`

## for restarting contaienrs with new images using watchtower

run the ansible playbook `ansible-playbook playbooks/update_containers.yml`

## KNOW ISSUE

if an error with mgs: `Error while fetching server API version: request() got an unexpected keyword argument 'chunked'`

then log into the VM, and run `pip install -U requests==2.26.0`.
