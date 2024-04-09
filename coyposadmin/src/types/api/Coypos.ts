export interface CoyposModel {
  version: string;
  time: Date | string;
  memory_used: number;
  memory_free: number;
  memory_total: number;
  os_platform: string;
  os_version: string;
  os_name: string;
  docker_container_id: string;
  uptime: string;
}
