# cloud native logging with k8s

This reposistory demonstrates how you can realize cloud native logging
(12-factor apps) using the following tools:
- k8s ochestrator used to host workloads (and optionally logging infrastructure)
- fluent-bit to collect workload logs (and optionally some log preprocessing)
- elk as a central store for your logs and access logs

# setup

This demo contains all the needed resources to create a local kind cluster. To
bootstrap the demo it's enough to execute the `setup-cluster.sh` script. When
the scripts completed the cluster should be up and running, this can be verified
using eg. `k9s`.

To access the elk stack you could use kubefwd to create the needed port
forwardings, eg.: `kubefwd svc -n elk-logging`. Once the forwarding are created,
you should be able to access kibana at: http://kibana:5601.

# known issues
- The ingress for kibana is currently broken.

