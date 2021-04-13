# Setup the local demonstration cluster
kind create cluster --name k8s-logs \
  --config kind-cluster-config.yaml

kubectl cluster-info --context kind-k8s-logs

kubectl apply -f \
  https://raw.githubusercontent.com/kubernetes/ingress-nginx/master/deploy/static/provider/kind/deploy.yaml

kubectl wait --namespace ingress-nginx \
  --for=condition=ready pod \
  --selector=app.kubernetes.io/component=controller \
  --timeout=90s

kubectl apply -f elk-logging-ns.yaml

kubectl apply -f elk-logging-elasticsearch.yaml
kubectl rollout status sts/es-cluster --namespace=elk-logging

kubectl apply -f elk-logging-kibana.yaml
kubectl rollout status deployment/kibana --namespace=elk-logging

kubectl apply -f elk-logging-logstash.yaml
kubectl rollout status deployment/logstash --namespace=elk-logging

kubectl apply -f elk-logging-ingress.yaml

kubectl apply -f kube-logging-ns.yaml
kubectl apply -f kube-logging-fluentbit.yaml
kubectl rollout status daemonset/fluent-bit -n kube-logging

# build and publish demo workload with esc common schema logging
cd demo-web
docker build . -t gebolze/demo-webapp
docker push gebolze/demo-webapp
cd ..

# deploy demo workload
kubectl apply -f demo-webapp.yaml
